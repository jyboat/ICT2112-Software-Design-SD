using Google.Cloud.Firestore;
using ClearCare.Models.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;
using ClearCare.Models.Control;
using Google.Api;
using System.ComponentModel;

namespace ClearCare.DataSource
{
    public class ServiceAppointmentGateway : IServiceAppointmentDB_Send
    {
        private readonly FirestoreDb _db;
        private IServiceAppointmentDB_Receive _receiver;
        
        private readonly IServiceType _iServiceType;
        
        public ServiceAppointmentGateway()
        {
            _db = FirebaseService.Initialize();
            _iServiceType = (IServiceType) new ServiceTypeManager();
        }

        // Property for setting the receiver after instantiation (Since gateway handle receiver callback - creates circular dependency. Break the cycle using property injection.)
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
                    ServiceAppointment appointment = ServiceAppointment.fromFirestoreData(document.Id, data);
                    appointmentList.Add(appointment);
                }
            }
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
            ServiceAppointment appointment = ServiceAppointment.fromFirestoreData(documentId, data);
            
            await _receiver.receiveServiceAppointmentById(appointment);
            return appointment;
        }

        public async Task<string> createAppointment(ServiceAppointment appointment)
        {
            // Get Collection in Firebase
            DocumentReference docRef = _db.Collection("ServiceAppointments").Document();

            // Convert input data to firestore data format for insert
            Dictionary<string, object> appointmentData = appointment.toFirestoreDictionary();

            // Overwrite field if it exists, otherwise create new field 
            await docRef.SetAsync(appointmentData);
            await _receiver.receiveCreatedServiceAppointmentId(docRef.Id);
            return docRef.Id;
        }

        public async Task<bool> updateAppointment(ServiceAppointment appointment)
        {
            // safely get the appointment id and validate it
            string appointmentId = appointment?.getAttribute("AppointmentId");
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
                Dictionary<string, object> appointmentData = appointment.toFirestoreDictionary();
                await docRef.SetAsync(appointmentData);
                _receiver.receiveUpdatedServiceAppointmentStatus(true);
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

        public async Task<bool> deleteAppointment(string appointmentId)
        {
            try
            {
                DocumentReference docRef = _db.Collection("ServiceAppointments").Document(appointmentId);

                // check if document exists
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (!snapshot.Exists)
                {
                    Console.WriteLine("Snapshot doesn't exist. APPTGATEWAY ID:");
                    Console.WriteLine(appointmentId);
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
                // Retrieve and convert Firestore timestamp to DateTime
                if (snapshot.ContainsField("DateTime"))
                {
                    Timestamp firestoreTimestamp = snapshot.GetValue<Timestamp>("DateTime");
                    DateTime appointmentTime = firestoreTimestamp.ToDateTime();

                    // Send the result via the receiver
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

        public async Task<(List<Dictionary<string, object>> appointments, Dictionary<string, string> patientNames)> fetchAllUnscheduledPatients()
        {
            var patients = new List<string>();
            var patientNameMap = new Dictionary<string, string>();
            
            Query patientQuery = _db.Collection("User")
                                    .WhereEqualTo("Role", "Patient");
            QuerySnapshot userPatientSnapshot = await patientQuery.GetSnapshotAsync();

            foreach(DocumentSnapshot document in userPatientSnapshot.Documents)
            {
                patients.Add(document.Id);
                patientNameMap[document.Id] = document.ContainsField("Name")
                                            ? document.GetValue<string>("Name"): "Unknown"; 
            }

            var unscheduledPatients = new List<ServiceAppointment>();
            var services = await _iServiceType.getServiceTypes();

            var serviceNames = services
                .Where(s => s.Status != "discontinued")
                .Select(s => s.Name)
                .ToList();

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
                var missingServices = serviceNames
                    .Where(serviceName => !existingServices.Contains(serviceName))
                    .ToList();

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
                .Select(appointment => appointment.toFirestoreDictionary())
                .ToList();

            return (result, patientNameMap);
        }
    }
}