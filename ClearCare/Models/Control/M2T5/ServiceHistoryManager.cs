using ClearCare.DataSource;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;

namespace ClearCare.Models.Control
{
    //TODO: add IUserList to display names for patient, nurse doctor?
    public class ServiceHistoryManager : IDatabaseObserver, IServiceHistory
    {
        private readonly ServiceHistoryMapper _ServiceHistoryMapper;

        public ServiceHistoryManager(ServiceHistoryMapper serviceHistoryMapper)
        {
            _ServiceHistoryMapper = serviceHistoryMapper;
            serviceHistoryMapper.attachObserver(this);
        }

        // GET ALL SERVICE HISTORY
        public async Task<List<Dictionary<string, object>>> getAllServiceHistory()
        {
            List<ServiceHistory> serviceHistoryList = await _ServiceHistoryMapper.fetchAllServiceHistory();
            List<Dictionary<string, object>> serviceHistoryDictionaryList = new();

            foreach (ServiceHistory history in serviceHistoryList)
            {
                Dictionary<string, object> historyDictionary = history.getServiceHistoryDetails();
                serviceHistoryDictionaryList.Add(historyDictionary);
            }

            return serviceHistoryDictionaryList;
        }

        // CREATE SERVICE HISTORY
        public async Task<string> createServiceHistory(string appointmentId, string serviceTypeId, string patientId, string nurseId, string doctorId, DateTime serviceDate, string location, string serviceOutcomes)
        {
            // Convert to UTC
            DateTime utcServiceDate = serviceDate.Kind == DateTimeKind.Utc ? serviceDate : serviceDate.ToUniversalTime();

            var serviceHistory = ServiceHistory.setServiceHistoryDetails(
                appointmentId, serviceTypeId, patientId, nurseId, doctorId, utcServiceDate, location, serviceOutcomes
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