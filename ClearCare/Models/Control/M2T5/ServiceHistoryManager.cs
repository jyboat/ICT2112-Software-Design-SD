using ClearCare.DataSource;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class ServiceHistoryManager : IDatabaseObserver, IServiceHistory
    {
        private readonly ServiceHistoryMapper _ServiceHistoryMapper;
        private readonly IUserList _UserList;

        public ServiceHistoryManager(ServiceHistoryMapper serviceHistoryMapper, IUserList userList)
        {
            _ServiceHistoryMapper = serviceHistoryMapper;
            _UserList = userList;
            serviceHistoryMapper.attachObserver(this);
        }

        // GET ALL SERVICE HISTORY
        public async Task<List<Dictionary<string, object>>> getAllServiceHistory(string userRole, string userId)
        {
            List<ServiceHistory> serviceHistoryList = await _ServiceHistoryMapper.fetchAllServiceHistory();
            List<Dictionary<string, object>> serviceHistoryDictionaryList = new();

            List<User> users = await _UserList.retrieveAllUsers();

            foreach (ServiceHistory history in serviceHistoryList)
            {
                Dictionary<string, object> historyDictionary = history.getServiceHistoryDetails();

                string patientId = historyDictionary["PatientId"].ToString();
                string nurseId = historyDictionary["NurseId"].ToString();
                string doctorId = historyDictionary["DoctorId"].ToString();

                // Filter based on role
                if ((userRole == "Nurse" && userId == nurseId) ||
                    (userRole == "Doctor" && userId == doctorId))
                {
                    // Find the matching user profiles
                    var patient = users.FirstOrDefault(u => u.getProfileData()["UserID"].ToString() == patientId);
                    var nurse = users.FirstOrDefault(u => u.getProfileData()["UserID"].ToString() == nurseId);
                    var doctor = users.FirstOrDefault(u => u.getProfileData()["UserID"].ToString() == doctorId);

                    // Replace IDs with names
                    historyDictionary["PatientName"] = patient != null ? patient.getProfileData()["Name"] : "Unknown";
                    historyDictionary["NurseName"] = nurse != null ? nurse.getProfileData()["Name"] : "Unknown";
                    historyDictionary["DoctorName"] = doctor != null ? doctor.getProfileData()["Name"] : "Unknown";

                    serviceHistoryDictionaryList.Add(historyDictionary);
                }
            }

            return serviceHistoryDictionaryList;
        }

        // CREATE SERVICE HISTORY
        public async Task<string> createServiceHistory(string appointmentId, string service, string patientId, string nurseId, string doctorId, DateTime serviceDate, string location, string serviceOutcomes)
        {
            // Convert to UTC
            DateTime utcServiceDate = serviceDate.Kind == DateTimeKind.Utc ? serviceDate : serviceDate.ToUniversalTime();

            var serviceHistory = ServiceHistory.setServiceHistoryDetails(
                appointmentId, service, patientId, nurseId, doctorId, utcServiceDate, location, serviceOutcomes
            );

            string serviceHistoryId = await _ServiceHistoryMapper.createServiceHistory(serviceHistory);
            return serviceHistoryId;
        }

        public void update(Subject subject, object data)
        {
            if (data is bool isSuccess)
            {
                if (isSuccess)
                {
                    Console.WriteLine("[ServiceHistoryManager] Service History created successfully.");
                }
                else
                {
                    Console.WriteLine("[ServiceHistoryManager] Service History creation failed.");
                }
            }
        }
    }
}