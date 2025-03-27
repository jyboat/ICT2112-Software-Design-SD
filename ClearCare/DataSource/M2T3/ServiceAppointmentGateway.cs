using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClearCare.Interfaces;
using Google.Api;
using System.ComponentModel;

namespace ClearCare.DataSource
{
    public class ServiceAppointmentGateway : IServiceAppointmentDB_Send
    {
        private readonly FirestoreDb _db;
        private IServiceAppointmentDB_Receive _receiver;
        public ServiceAppointmentGateway()
        {
            _db = FirebaseService.Initialize();
        }

        // Property for setting the receiver after instantiation (Since gateway handle receiver callback - creates circular dependency. SO need break cycle by property injection)
        public IServiceAppointmentDB_Receive Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        public async Task<List<ServiceAppointment>> fetchAllServiceAppointments()
        {
            CollectionReference appointmentsRef = _db.Collection("ServiceAppointments");
            QuerySnapshot snapshot = await appointmentsRef.GetSnapshotAsync();
            List<ServiceAppointment> appointmentList = new List<ServiceAppointment>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var data = document.ToDictionary();
                    ServiceAppointment appointment = ServiceAppointment.FromFirestoreData(document.Id, data);
                    appointmentList.Add(appointment);
                }
            }
            await CheckAndUpdateStatusAsync(appointmentList);
            await _receiver.receiveServiceAppointmentList(appointmentList);
            return appointmentList;
        }

        public async Task<ServiceAppointment> fetchServiceAppointmentByID(string documentId)
        {
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Firestore Document Not Found: {documentId}");
                return null;
            }
            // Convert snapshot to dictionary
            var data = snapshot.ToDictionary();

            // Convert Firestore document to a ServiceAppointment object
            ServiceAppointment appointment = ServiceAppointment.FromFirestoreData(documentId, data);

            // ✅ Pass appointment (not snapshot) to CheckAndUpdateStatusAsync()
            appointment = await CheckAndUpdateStatusAsync(appointment);
            
            await _receiver.receiveServiceAppointmentById(appointment);
            return appointment;
        }

        public async Task<string> CreateAppointment(ServiceAppointment appointment)
        {
            // Get Collection in Firebase
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document();

            // Convert input data to firestore data format for insert
            Dictionary<string, object> appointmentData = appointment.ToFirestoreDictionary();

            // Overwrite field if exist, create new if doesn't
            await docRef.SetAsync(appointmentData);

            await _receiver.receiveCreatedServiceAppointmentId(docRef.Id);

            return docRef.Id;
        }

        public async Task<bool> UpdateAppointment(ServiceAppointment appointment)
        {
            // debug the incoming data
            string appointmentJson = JsonConvert.SerializeObject(appointment);
            Console.WriteLine($"attempting to update appointment with data: {appointmentJson}");

            // safely get the appointment id and validate it
            string appointmentId = appointment?.GetAttribute("AppointmentId");
            Console.WriteLine($"extracted appointmentId: '{appointmentId}'");

            if (string.IsNullOrEmpty(appointmentId))
            {
                Console.WriteLine("failed: appointmentId is null or empty");
                _receiver.receiveUpdatedServiceAppointmentStatus(false);
                return false;
            }

            try
            {
                // validate document reference creation
                DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);
                Console.WriteLine($"document path: {docRef.Path}");

                // check if document exists
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists)
                {
                    Console.WriteLine($"document with id '{appointmentId}' doesn't exist");
                    _receiver.receiveUpdatedServiceAppointmentStatus(false);
                    return false;
                }

                // update with the new data
                Dictionary<string, object> appointmentData = appointment.ToFirestoreDictionary();
                Console.WriteLine($"converted data for firestore: {JsonConvert.SerializeObject(appointmentData)}");
                await docRef.SetAsync(appointmentData);
                _receiver.receiveUpdatedServiceAppointmentStatus(true);
                Console.WriteLine("update successful");
                return true;
            }
            catch (Exception ex)
            {
                // detailed error logging
                string errorDetails = $"error updating appointment '{appointmentId}' in firestore: {ex.Message}";
                errorDetails += $"\nstack trace: {ex.StackTrace}";

                if (ex.InnerException != null)
                {
                    errorDetails += $"\ninner exception: {ex.InnerException.Message}";
                }

                errorDetails += $"\nappointment data: {appointmentJson}";

                Console.WriteLine(errorDetails);
                _receiver.receiveUpdatedServiceAppointmentStatus(false);
                return false;
            }
        }

        public async Task<bool> DeleteAppointment(string appointmentId)
        {
            try
            {
                DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);

                // check if document exists
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists)
                {
                    await _receiver.receiveDeletedServiceAppointmentStatus(false);
                    return false;
                }

                // delete doc
                await docRef.DeleteAsync();

                await _receiver.receiveDeletedServiceAppointmentStatus(true);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"error deleting appointment in firestore: {e.Message}");
                await _receiver.receiveDeletedServiceAppointmentStatus(false);
                throw;
            }
        }
        public async Task<DateTime?> fetchAppointmentTime(string appointmentId)
        {
            // Get Firestore document reference
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Console.WriteLine($"Firestore Document Not Found: {appointmentId}");
                return null;
            }

            try
            {
                // ✅ Retrieve and convert Firestore timestamp to DateTime
                if (snapshot.ContainsField("DateTime"))
                {
                    Timestamp firestoreTimestamp = snapshot.GetValue<Timestamp>("DateTime");
                    DateTime appointmentTime = firestoreTimestamp.ToDateTime();

                    // ✅ Send the result via the receiver
                    await _receiver.receiveServiceAppointmentTimeById(appointmentTime);

                    return appointmentTime;
                }
                else
                {
                    Console.WriteLine($"Field 'DateTime' not found in document: {appointmentId}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error retrieving appointment time: {e.Message}");
                return null;
            }
        }

        private async Task<List<ServiceAppointment>> CheckAndUpdateStatusAsync(List<ServiceAppointment> appointments)
        {
            if (appointments == null || appointments.Count == 0) return new List<ServiceAppointment>();

            foreach (var appointment in appointments)
            {
                if (appointment.CheckAndMarkAsMissed())
                {
                    Console.WriteLine($"🔍 Appointment ID: {appointment.GetAttribute("AppointmentId")}, Status: {appointment.GetAttribute("Status")}, DateTime: {appointment.GetAttribute("Datetime")}");

                    bool success = await UpdateAppointment(appointment); // 🔥 Await the async method
                    if (!success)
                    {
                        Console.WriteLine($"Failed to update appointment status to missed: {appointment.GetAttribute("AppointmentId")}");
                    }
                    else
                    {
                        Console.WriteLine($"updated {appointment.GetAttribute("AppointmentId")} to {appointment.GetAttribute("Status")}");
                    }

                }

            }

            return appointments;
        }

        private async Task<ServiceAppointment> CheckAndUpdateStatusAsync(ServiceAppointment appointment)
        {
            if (appointment.CheckAndMarkAsMissed())
            {
                bool success = await UpdateAppointment(appointment);
                if (!success)
                {
                    Console.WriteLine($"Failed to update appointment status to missed: {appointment.GetAttribute("AppointmentId")}");
                }
                else
                {
                    Console.WriteLine($"updated {appointment.GetAttribute("AppointmentId")} to {appointment.GetAttribute("Status")}");
                }
            }
            return appointment;
        }

        public class Patient
        {
            public string PatientId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        // To be changed delete once got interface from other teams
        public async Task<List<string>> getAllServices()
        {
            Query serviceQuery = _db.Collection("service_type");
            QuerySnapshot snapshot2 = await serviceQuery.GetSnapshotAsync();

            var services = new List<string>();

            foreach (DocumentSnapshot document in snapshot2.Documents)
            {
                services.Add(document.GetValue<string>("name"));
            }
            return services;
        }

        // To do: Retrieve from firebase/interface
        public async Task<List<Dictionary<string, object>>> fetchAllUnscheduledPatients()
        {

            // TODO: Change to interface
            var patients = new List<string>();
            
            Query patientQuery = _db.Collection("User")
                                    .WhereEqualTo("Role", "Patient");
            QuerySnapshot userPatientSnapshot = await patientQuery.GetSnapshotAsync();

            foreach(DocumentSnapshot document in userPatientSnapshot.Documents)
            {
                patients.Add(document.Id);
            }

            var unscheduledPatients = new List<ServiceAppointment>();
            var services = await getAllServices();

            foreach (var patient in patients)
            {
                // Query to get all the appointments of this patient
                Query appointmentsRef = _db.Collection("ServiceAppointments")
                                            .WhereEqualTo("PatientId", patient);
                QuerySnapshot snapshot = await appointmentsRef.GetSnapshotAsync();

                // Get all services this patient already has
                var existingServices = snapshot.Documents
                                               .Select(doc => doc.GetValue<string>("Service"))
                                               .ToList();

                // Find missing services (those that exist in the service list but not in the existing appointments)
                var missingServices = services.Where(service => !existingServices.Contains(service)).ToList();

                // For each missing service, create a new service appointment
                foreach (var service in missingServices)
                {
                    unscheduledPatients.Add(ServiceAppointment.setApptDetails(
                        patientId: patient,
                        nurseId: "",
                        doctorId: "",
                        Service: service,
                        status: "Pending",
                        dateTime: DateTime.UtcNow,
                        slot: 0,
                        location: "Physical"
                    ));
                }
            }

            // Convert the appointments to Firestore dictionaries
            List<Dictionary<string, object>> result = unscheduledPatients
                .Select(appointment => appointment.ToFirestoreDictionary())
                .ToList();

            return result;
        }

    }

    // {
    //     private readonly FirestoreDb _db;

    //     public ServiceAppointmentGateway()
    //     {
    //         // Initialize Firebase
    //         _db = FirebaseService.Initialize();
    //     }

    //     // Create a new appointment
    //     public async Task<string> CreateAppointmentAsync(ServiceAppointment appointment)
    //     {
    //         // Get Collection in Firebase
    //         DocumentReference docRef = _db.Collection("ServiceAppointments").Document();

    //         appointment.SetAppointmentId(docRef.Id);

    //         // Convert input data to firestore data format for insert
    //         Dictionary<string, object> appointmentData = appointment.ToFirestoreDictionary();

    //         // Overwrite field if exist, create new if doesn't
    //         await docRef.SetAsync(appointmentData);

    //         return docRef.Id;
    //     }

    //     // Retrieve all
    //     public async Task<List<ServiceAppointment>> GetAllAppointmentsAsync()
    //     {
    //         CollectionReference appointmentsRef = _db.Collection("ServiceAppointments");

    //         QuerySnapshot snapshot = await appointmentsRef.GetSnapshotAsync();
    //         List<ServiceAppointment> appointmentList = new List<ServiceAppointment>();

    //         foreach (DocumentSnapshot document in snapshot.Documents)
    //         {
    //             if (document.Exists)
    //             {
    //                 var data = document.ToDictionary();
    //                 ServiceAppointment appointment = ServiceAppointment.FromFirestoreData(document.Id, data);
    //                 appointmentList.Add(appointment);
    //             }
    //         }

    //         return appointmentList;
    //     }

    //     // Retrieve an appointment by ID
    //     public async Task<ServiceAppointment?> GetAppointmentByIdAsync(string appointmentId)
    //     {
    //         DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);
    //         DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

    //         if (!snapshot.Exists)
    //         {
    //             Console.WriteLine($"Firestore Document Not Found: {appointmentId}");
    //             return null;
    //         }

    //         // Convert to Dictionary from firebase key-value format
    //         var data = snapshot.ToDictionary();
    //         return ServiceAppointment.FromFirestoreData(appointmentId, data);
    //     }

    //     // update an existing appointment
    //     public async Task<bool> UpdateAppointmentAsync(ServiceAppointment appointment)
    //     {
    //         try
    //         {
    //             DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointment.GetAttribute("AppointmentId"));

    //             // check if document exists
    //             DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
    //             if (!snapshot.Exists)
    //             {
    //                 return false;
    //             }

    //             // update with the new data
    //             Dictionary<string, object> appointmentData = appointment.ToFirestoreDictionary();
    //             await docRef.SetAsync(appointmentData);

    //             return true;
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"error updating appointment in firestore: {ex.Message}");
    //             return false;
    //         }
    //     }

    //     // delete an existing appointment
    //     public async Task<bool> DeleteAppointmentAsync(string appointmentId)
    //     {
    //         try
    //         {
    //             DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);

    //             // check if document exists
    //             DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
    //             if (!snapshot.Exists)
    //             {
    //                 return false;
    //             }

    //             // delete doc
    //             await docRef.DeleteAsync();

    //             return true;
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine($"error deleting appointment in firestore: {e.Message}");
    //             throw;
    //         }
    //     }
    // }
}