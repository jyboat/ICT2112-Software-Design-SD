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
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class AutomaticAppointmentScheduler : AbstractSchedulingNotifier
    {
        // Interfaces Automatic Requires
        private readonly ICreateAppointment _iCreateAppointment;
        private readonly INurseAvailability _iNurseAvailability;
        private readonly INotification _iNotification;
        private IAutomaticScheduleStrategy? _iAutomaticScheduleStrategy;
        private readonly IServiceType _iServiceType;
        private FirestoreDb db;
        // private readonly ServiceBacklogManagement _serviceBacklogManagement;

        // // Declare the field at the class level
        // private readonly ServiceAppointmentGateway _serviceAppointmentGateway;

        // Constructor initializes the field
        public AutomaticAppointmentScheduler(
            // ICreateAppointment ICreateAppointment, 
            // INurseAvailability INurseAvailability,
            // INotification INotification,
            // IServiceType IServiceType,
            IAutomaticScheduleStrategy? IAutomaticScheduleStrategy = null
            )
        {
            _iCreateAppointment = new ServiceAppointmentManagement();
            _iNurseAvailability = new NurseAvailabilityManagement();
            _iNotification = new NotificationManager();
            _iServiceType = new ServiceTypeManager();;
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
        // public class Nurse 
        // {
        //     public string NurseId { get; set; } = string.Empty;
        //     public string Name { get; set; } = string.Empty;
        // }

        public class Patient
        {
            public string PatientId { get; set; }  = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        // To Do: In ServiceAppointmentGateway, PreferredNurseStrategy and this class
        // To Do: Modify HTML to showcase how auto scheduling works
        // To Do: Add more interface for serviceAppointment db operations

        public async void AutomaticallyScheduleAppointment(List<ServiceAppointment> unscheduledAppointment)
        {
            var timeslot = new Dictionary<int, DateTime>
            {
                { 1, DateTime.Parse("4:00 PM").ToUniversalTime()  },
                { 2, DateTime.Parse("5:00 PM").ToUniversalTime()  },
                { 3, DateTime.Parse("6:00 PM").ToUniversalTime()  },
                { 4, DateTime.Parse("7:00 PM").ToUniversalTime()  },
                { 5, DateTime.Parse("9:00 PM").ToUniversalTime()  },
                { 6, DateTime.Parse("10:00 PM").ToUniversalTime()  },
                { 7, DateTime.Parse("11:00 PM").ToUniversalTime()  }
            };

            // Attach listener only when scheduling is called
            var _serviceBacklogManagement = new ServiceBacklogManagement();
            attach(_serviceBacklogManagement);

            if (_iAutomaticScheduleStrategy == null)
            {
                throw new InvalidOperationException("Scheduling strategy has not been set. Use SetAlgorithm() first.");
            }

            var serviceNames = await _iServiceType.GetServiceTypes();
            var services = serviceNames.Select(service => service.Name).ToList();

            var serviceSlotTracker = new Dictionary<string, Dictionary<int, int>>();
            foreach(var service in services){
                Query serviceList = db.Collection("ServiceAppointments")
                                    .WhereEqualTo("Service", service);
                                    // .WhereArrayContains("DateTime", nurse.NurseId);
                QuerySnapshot serviceSnapshot = await serviceList.GetSnapshotAsync();
                foreach(DocumentSnapshot document in serviceSnapshot.Documents){
                    DateTime appointmentDateTime = document.GetValue<DateTime>("DateTime");
                    DateTime appointmentDate = appointmentDateTime.Date;
                    DateTime todayDate = DateTime.Today;

                    if(appointmentDate == todayDate){
                        string serviceName = document.GetValue<string>("Service");
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
            // var nurses = new List<string>();

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

            var nurses = new List<string>();

            Query userQuery = db.Collection("User").WhereEqualTo("Role", "Nurse");
            QuerySnapshot userSnapshot = await userQuery.GetSnapshotAsync();

            foreach(DocumentSnapshot document in userSnapshot.Documents)
            {
                nurses.Add(document.Id);
            }

            foreach(var nurse in nurses){
                Query nurseSlotList = db.Collection("ServiceAppointments")
                                    .WhereEqualTo("NurseId", nurse);
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
                        // Console.WriteLine($"Data1: {appointmentDate}");
                        // Console.WriteLine($"Data2: {todayDate}");
                    }
                    else{
                        continue;
                    }
                }
            }

            var patientSlotTracker = new Dictionary<string, List<int>>();

            var patients = new List<string>();
            
            Query patientQuery = db.Collection("User").WhereEqualTo("Role", "Patient");;
            QuerySnapshot userPatientSnapshot = await patientQuery.GetSnapshotAsync();

            foreach(DocumentSnapshot document in userPatientSnapshot.Documents)
            {
                patients.Add(document.Id);
            }

            foreach(var patient in patients){
                Query patientSlotList = db.Collection("ServiceAppointments")
                                    .WhereEqualTo("PatientId", patient);
                QuerySnapshot patientSnapshot = await patientSlotList.GetSnapshotAsync();
                foreach(DocumentSnapshot document in patientSnapshot.Documents){
                    DateTime appointmentDateTime = document.GetValue<DateTime>("DateTime");
                    DateTime appointmentDate = appointmentDateTime.Date;
                    DateTime todayDate = DateTime.Today;
                    
                    if(appointmentDate == todayDate){
                        string patientId = document.GetValue<string>("PatientId");
                        int slot = document.GetValue<int>("Slot");

                        // Create list for the nurse if doesn't exists
                        if (!patientSlotTracker.ContainsKey(patientId))
                        {
                            patientSlotTracker[patientId] = new List<int>();
                        }
                        // Add the slot number into the list.
                        patientSlotTracker[patientId].Add(slot);
                        // Console.WriteLine($"Data1: {appointmentDate}");
                        // Console.WriteLine($"Data2: {todayDate}");
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
                    appointment.GetAttribute("Service"), 
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
                unscheduledAppointment,
                nurses, 
                services, 
                backlogEntries,
                patientSlotTracker,
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
                        serviceAppt.GetAttribute("Service"),
                        "Scheduled",
                        timeslot[serviceAppt.GetIntAttribute("Slot")],
                        serviceAppt.GetIntAttribute("Slot"),
                        "Physical"
                    );

                    var message = "Your Appointment at";

                    await _iNotification.createNotification("USR22"
                    , message);

                    // Console.WriteLine($"Successfully rescheduled Appointment: {serviceAppt.GetAttribute("AppointmentId")}");
                    notify(serviceAppt.GetAttribute("AppointmentId"), "success");
                }
                else{
                    // For new appointments
                    var appointmentId = await _iCreateAppointment.CreateAppointment(
                        serviceAppt.GetAttribute("PatientId"),
                        serviceAppt.GetAttribute("NurseId"),
                        "Hardcode Doctor",
                        serviceAppt.GetAttribute("Service"),
                        "Backlog",
                        timeslot[serviceAppt.GetIntAttribute("Slot")],
                        serviceAppt.GetIntAttribute("Slot"),
                        "Physical"
                    );

                    var message = "Your Appointment at";

                    await _iNotification.createNotification("USR22"
                    , message);

                    // When appointment isn't scheduled
                    if (string.IsNullOrEmpty(serviceAppt.GetAttribute("NurseId")))
                    {
                        Console.WriteLine($"Failed to schedule Appointment: {appointmentId}");
                        notify(appointmentId, "fail");
                    }
                }
            }
        }

        // public async Task TestInterface()
        // {
        //    //  await _iCreateAppointment.CreateAppointment();
        //     var staffAvailability = await _iNurseAvailability.getAllStaffAvailability();
        // }   

    }
}