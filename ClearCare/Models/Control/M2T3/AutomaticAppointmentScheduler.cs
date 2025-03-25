using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using Google.Protobuf.WellKnownTypes;
using ClearCare.Interfaces;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface.M2T3;
using System.Reflection.Metadata;

namespace ClearCare.Models.Control
{
    public class AutomaticAppointmentScheduler : AbstractSchedulingNotifier
    {
        // Interfaces Automatic Requires
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        private IAutomaticScheduleStrategy? _iAutomaticScheduleStrategy;
        private FirestoreDb db;
        // private readonly ServiceBacklogManagement _serviceBacklogManagement;

        // // Declare the field at the class level
        // private readonly ServiceAppointmentGateway _serviceAppointmentGateway;

        // Constructor initializes the field
        public AutomaticAppointmentScheduler(
            ICreateAppointment ICreateAppointment, 
            INurseAvailability INurseAvailability,
            IAutomaticScheduleStrategy? IAutomaticScheduleStrategy = null)
        {
            _iCreateAppointment = ICreateAppointment;
            _iNurseAvailability = INurseAvailability;
            // To be set at runtime later
            _iAutomaticScheduleStrategy = IAutomaticScheduleStrategy; 
            db = FirebaseService.Initialize();

            // _serviceBacklogManagement = new ServiceBacklogManagement();
            // attach(_serviceBacklogManagement);
        }

        public void SetAlgorithm(IAutomaticScheduleStrategy IAutomaticScheduleStrategy)
        {
            _iAutomaticScheduleStrategy = IAutomaticScheduleStrategy; 
        }

        // Dummy Entity for testing
        public class Nurse 
        {
            public string NurseId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        // public class Patient
        // {
        //     public string PatientId { get; set; }  = string.Empty;
        //     public string Name { get; set; } = string.Empty;
        // }

        // public List<Patient> getPatients(){
        //     var patients = new List<Patient>
        //     {
        //         new Patient { PatientId = "PAT001", Name = "Patient 1" },
        //         new Patient { PatientId = "PAT002", Name = "Patient 2" },
        //         new Patient { PatientId = "PAT003", Name = "Patient 3" }
        //     };
            
        //     return patients; 
        // }

        // To Do: In ServiceAppointmentGateway, PreferredNurseStrategy and this class
        // To Do: Modify HTML to showcase how auto scheduling works
        // To Do: Add more interface for serviceAppointment db operations

        public async void AutomaticallyScheduleAppointment(List<string> patients)
        {
            // Attach listener only when scheduling is called
            var _serviceBacklogManagement = new ServiceBacklogManagement();
            attach(_serviceBacklogManagement);

            if (_iAutomaticScheduleStrategy == null)
            {
                throw new InvalidOperationException("Scheduling strategy has not been set. Use SetAlgorithm() first.");
            }

            // To do: Use interface to get services
            Query serviceQuery = db.Collection("service_type");
            QuerySnapshot snapshot2 = await serviceQuery.GetSnapshotAsync();
            
            var services = new List<string>();

            foreach(DocumentSnapshot document in snapshot2.Documents)
            {
                services.Add(document.GetValue<string>("name"));
            }

            var serviceSlotTracker = new Dictionary<string, Dictionary<int, int>>();
            foreach(var service in services){
                Query serviceList = db.Collection("ServiceAppointments")
                                    .WhereEqualTo("ServiceTypeId", service);
                                    // .WhereArrayContains("DateTime", nurse.NurseId);
                QuerySnapshot serviceSnapshot = await serviceList.GetSnapshotAsync();
                foreach(DocumentSnapshot document in serviceSnapshot.Documents){
                    DateTime appointmentDateTime = document.GetValue<DateTime>("DateTime");
                    DateTime appointmentDate = appointmentDateTime.Date;
                    DateTime todayDate = DateTime.Today;

                    if(appointmentDate == todayDate){
                        string serviceName = document.GetValue<string>("ServiceTypeId");
                        int slot = document.GetValue<int>("Slot");

                        // Create list for the nurse if doesn't exists
                        if (!serviceSlotTracker.ContainsKey(serviceName))
                        {
                            serviceSlotTracker[serviceName] = new Dictionary<int, int>();
                        }
                        if (!serviceSlotTracker[serviceName].ContainsKey(slot))
                        {
                            // Initialize to 0
                            serviceSlotTracker[serviceName][slot] = 0; 
                        }
                        // Increment the amount of slot
                        // Each time slot can have 2 patients 
                        serviceSlotTracker[serviceName][slot]++;
                    }
                    else{
                        continue;
                    }
                }
            }
            
            // To Do: Need check whether they're from pre-discharge department
            // var nurses = new List<Nurse>();

            // var AvailableNurse = await _iNurseAvailability.getAllStaffAvailability();

            // foreach (var nurse in AvailableNurse)
            // {
            //     var availabilityDetails = nurse.getAvailabilityDetails();
            //     if (availabilityDetails.ContainsKey("nurseID"))
            //     {
            //         nurses.Add(new Nurse
            //         {
            //             NurseId = availabilityDetails["nurseID"].ToString() ?? " "    
            //         });
            //     }
            // }
            
            // var serviceSlotTracker = new Dictionary<string, Dictionary<int, int>>();
            var nurseSlotTracker = new Dictionary<string, List<int>>();

            // To do: Use interface for nurse
            var nurses = new List<Nurse>
            {
                new Nurse { NurseId = "NURSE001", Name = "Nurse A" },
                new Nurse { NurseId = "NURSE002", Name = "Nurse B" },
                new Nurse { NurseId = "NURSE003", Name = "Nurse C" },
                new Nurse { NurseId = "NURSE004", Name = "Nurse D" },
                new Nurse { NurseId = "NURSE005", Name = "Nurse E" },
                new Nurse { NurseId = "NURSE006", Name = "Nurse F" }
            };

            foreach(var nurse in nurses){
                Query nurseSlotList = db.Collection("ServiceAppointments")
                                    .WhereEqualTo("NurseId", nurse.NurseId);
                QuerySnapshot patientSnapshot = await nurseSlotList.GetSnapshotAsync();
                foreach(DocumentSnapshot document in patientSnapshot.Documents){
                    DateTime appointmentDateTime = document.GetValue<DateTime>("DateTime");
                    DateTime appointmentDate = appointmentDateTime.Date;
                    DateTime todayDate = DateTime.Today;
                    
                    if(appointmentDate == todayDate){
                        string nurseID = document.GetValue<string>("NurseId");
                        int slot = document.GetValue<int>("Slot");

                        // Create list for the nurse if doesn't exists
                        if (!nurseSlotTracker.ContainsKey(nurseID))
                        {
                            nurseSlotTracker[nurseID] = new List<int>();
                        }
                        // Add the slot number into the list.
                        nurseSlotTracker[nurseID].Add(slot);
                        Console.WriteLine($"Data1: {appointmentDate}");
                        Console.WriteLine($"Data2: {todayDate}");
                    }
                    else{
                        continue;
                    }
                }
            }

            // Hardcoded data
            var backlogEntries = new List<ServiceAppointment>();

            Query BLQuery = db.Collection("ServiceBacklogs");
            QuerySnapshot snapshot = await BLQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                string appointmentId = document.GetValue<string>("appointmentId");
                ServiceAppointment appointment = await _iCreateAppointment.getAppointmentByID(appointmentId);
                appointment.updateServiceAppointementById(
                    appointment, 
                    appointment.GetAttribute("PatientId"), 
                    appointment.GetAttribute("NurseId"), 
                    appointment.GetAttribute("DoctorId"), 
                    appointment.GetAttribute("ServiceTypeId"), 
                    appointment.GetAttribute("Status"), 
                    DateTime.Now, 
                    Convert.ToInt32(appointment.GetAttribute("Slot")), 
                    appointment.GetAttribute("Location")
                );
                // entry["DateTime"] = DateTime.Now; 
                // ServiceAppointment appointment = ServiceAppointment.FromFirestoreData(appointmentId, entry);

                backlogEntries.Add(appointment);
            }

            // Call the auto-assignment function
            var serviceAppointment = _iAutomaticScheduleStrategy.AutomaticallySchedule(
                nurses, 
                patients, 
                services, 
                backlogEntries,
                serviceSlotTracker,
                nurseSlotTracker);

            foreach (var serviceAppt in serviceAppointment)
            {
                if (serviceAppt.GetAttribute("AppointmentId") != "")
                {
                    // For when backlog is successfully rescheduled
                    await _iCreateAppointment.DeleteAppointment(
                        serviceAppt.GetAttribute("AppointmentId")
                    );

                    await _iCreateAppointment.CreateAppointment(
                        serviceAppt.GetAttribute("PatientId"),
                        serviceAppt.GetAttribute("NurseId"),
                        "Hardcode Doctor",
                        serviceAppt.GetAttribute("ServiceTypeId"),
                        "Scheduled",
                        DateTime.UtcNow,
                        serviceAppt.GetIntAttribute("Slot"),
                        "Physical"
                    );

                    // Console.WriteLine($"Successfully rescheduled Appointment: {serviceAppt.GetAttribute("AppointmentId")}");
                    notify(serviceAppt.GetAttribute("AppointmentId"), "success");
                }
                else{
                    // For new appointments
                    var appointmentId = await _iCreateAppointment.CreateAppointment(
                        serviceAppt.GetAttribute("PatientId"),
                        serviceAppt.GetAttribute("NurseId"),
                        "Hardcode Doctor",
                        serviceAppt.GetAttribute("ServiceTypeId"),
                        "Backlog",
                        DateTime.UtcNow,
                        serviceAppt.GetIntAttribute("Slot"),
                        "Physical"
                    );

                    // When appointment isn't scheduled
                    if (string.IsNullOrEmpty(serviceAppt.GetAttribute("NurseId")))
                    {
                        Console.WriteLine($"Failed to schedule Appointment: {appointmentId}");
                        notify(appointmentId, "fail");
                    }
                }
            }
        }

        public async Task TestInterface()
        {
           //  await _iCreateAppointment.CreateAppointment();
            var staffAvailability = await _iNurseAvailability.getAllStaffAvailability();
        }   

    }
}