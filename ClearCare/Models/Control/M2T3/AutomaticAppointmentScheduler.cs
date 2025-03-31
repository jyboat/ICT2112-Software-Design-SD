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
         private readonly IServiceType _iServiceType;
        private readonly IRetrieveAllAppointments _iRetrieveAppointment;
        private readonly IBacklogAppointments _iBacklogAppointment;
        private readonly IUserList _iUserList;
        private IAutomaticScheduleStrategy? _iAutomaticScheduleStrategy;
       
        private FirestoreDb db;
        // private readonly ServiceBacklogManagement _serviceBacklogManagement;

        // Constructor initializes the field
        public AutomaticAppointmentScheduler(
            IAutomaticScheduleStrategy? IAutomaticScheduleStrategy = null
            )
        {
            _iCreateAppointment = (ICreateAppointment) new ServiceAppointmentManagement();
            _iNurseAvailability = (INurseAvailability) new NurseAvailabilityManagement();
            _iNotification = (INotification) new NotificationManager();
            _iServiceType = (IServiceType) new ServiceTypeManager();
            _iRetrieveAppointment = (IRetrieveAllAppointments) new ServiceAppointmentStatusManagement();
            _iBacklogAppointment = (IBacklogAppointments) new ServiceBacklogManagement();
            var userGateway = new UserGateway();
            _iUserList = (IUserList) new AdminManagement(userGateway);
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

        public async Task<List<ServiceAppointment>> AutomaticallyScheduleAppointment(List<ServiceAppointment> unscheduledAppointment)
        {
            var userList = await _iUserList.retrieveAllUsers();
            var timeslot = new Dictionary<int, DateTime>
            {
                { 0, DateTime.Parse("8:00 AM").ToUniversalTime()  },
                { 1, DateTime.Parse("4:00 PM").ToUniversalTime()  },
                { 2, DateTime.Parse("5:00 PM").ToUniversalTime()  },
                { 3, DateTime.Parse("6:00 PM").ToUniversalTime()  },
                { 4, DateTime.Parse("7:00 PM").ToUniversalTime()  },
                { 5, DateTime.Parse("9:00 PM").ToUniversalTime()  },
                { 6, DateTime.Parse("10:00 PM").ToUniversalTime()  },
                { 7, DateTime.Parse("11:00 PM").ToUniversalTime()  },
                { 8, DateTime.Parse("12:00 PM").ToUniversalTime()  }
            };

            // Attach listener only when scheduling is called
            var _serviceBacklogManagement = new ServiceBacklogManagement();
            attach(_serviceBacklogManagement);

            if (_iAutomaticScheduleStrategy == null)
            {
                throw new InvalidOperationException("Scheduling strategy has not been set. Use SetAlgorithm() first.");
            }

            var serviceNames = await _iServiceType.GetServiceTypes();
            var servicesModality = serviceNames.ToDictionary(service => service.Name, service => service.Modality);
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
            
            // var serviceSlotTracker = new Dictionary<string, Dictionary<int, int>>();
            var nurseSlotTracker = new Dictionary<string, List<int>>();
                        
            // Grab all nurses that's available today
            var nurses = new List<string>();

            var AvailableNurse = await _iNurseAvailability.getAllStaffAvailability();

            foreach (var nurse in AvailableNurse)
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");
                var availabilityDetails = nurse.getAvailabilityDetails();
                if (availabilityDetails.ContainsKey("nurseID") && availabilityDetails.ContainsKey("date"))
                {
                    var date = availabilityDetails["date"].ToString();

                    if (date == today)
                    {
                        nurses.Add(availabilityDetails["nurseID"]?.ToString() ?? "");
                    }
                }
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
                    }
                    else{
                        continue;
                    }
                }
            }

            var patientSlotTracker = new Dictionary<string, List<int>>();
            var patients = userList
                .Where(p => p.getProfileData()["Role"].ToString()?.ToLower() == "patient")
                .Select(p => p.getProfileData()["UserID"].ToString())
                .ToList();

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
                    }
                    else{
                        continue;
                    }
                }
            }

            var backlogEntries = new List<ServiceAppointment>();

            var backlogIDs = await  _iBacklogAppointment.getAllBacklogAppointmentID();

            // Use appointmentID in backlog to retrieve appointment details for re-scheduling
            foreach(var backlog in backlogIDs){
                string appointmentId = backlog;
                ServiceAppointment appointment = await _iRetrieveAppointment.getServiceAppointmentById(appointmentId);
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
                        servicesModality[serviceAppt.GetAttribute("Service")]
                    );

                    var message = "Your Appointment for " +
                                serviceAppt.GetAttribute("Service") +
                                "has been scheduled at " +
                                timeslot[serviceAppt.GetIntAttribute("Slot")].ToString() +
                                "Location: "+ servicesModality[serviceAppt.GetAttribute("Service")].ToString();

                    await _iNotification.createNotification(serviceAppt.GetAttribute("PatientId"), message);

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
                        "Scheduled",
                        timeslot[serviceAppt.GetIntAttribute("Slot")],
                        serviceAppt.GetIntAttribute("Slot"),
                        servicesModality[serviceAppt.GetAttribute("Service")]
                    );

                    var message = "Your Appointment for " +
                                serviceAppt.GetAttribute("Service") +
                                "has been scheduled at " +
                                timeslot[serviceAppt.GetIntAttribute("Slot")].ToString() +
                                "Location: "+ servicesModality[serviceAppt.GetAttribute("Service")].ToString();

                    await _iNotification.createNotification(serviceAppt.GetAttribute("PatientId"), message);

                    // When appointment isn't scheduled
                    if (string.IsNullOrEmpty(serviceAppt.GetAttribute("NurseId")))
                    {
                        Console.WriteLine($"Failed to schedule Appointment: {appointmentId}");
                        notify(appointmentId, "fail");
                    }
                }
            }

            return serviceAppointment;
        }

    }
}