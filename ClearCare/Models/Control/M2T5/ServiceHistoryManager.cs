using ClearCare.DataSource;
using ClearCare.Models.Entities;
using ClearCare.Interfaces;

namespace ClearCare.Models.Control
{
    // to add IUserList
    public class ServiceHistoryManager : IDatabaseObserver, IServiceHistory
    {
        private readonly ServiceHistoryMapper _ServiceHistoryMapper;
        private readonly IAppointmentStatus _ApptStatusService;

        public ServiceHistoryManager(ServiceHistoryMapper serviceHistoryMapper, IAppointmentStatus apptStatusService)
        {
            _ServiceHistoryMapper = serviceHistoryMapper;
            _ApptStatusService = apptStatusService;
            // serviceHistoryMapper.attachObserver(this); // Attach Manager as an Observer
        }

        // GET ALL SERVICE HISTORY
        public async Task<List<Dictionary<string, object>>> getAllServiceHistory()
        {
            List<ServiceHistory> serviceHistoryList = await _ServiceHistoryMapper.fetchAllServiceHistory();
            List<Dictionary<string, object>> serviceHistoryDictionaryList = new List<Dictionary<string, object>>();

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
            if (data is ServiceHistory serviceHistory)
            {
                Console.WriteLine($"Manage received DB update: {serviceHistory}");
            }
        }
    }
}