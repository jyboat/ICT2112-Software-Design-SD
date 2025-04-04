using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class ServiceHistory
    {
        [FirestoreProperty]
        private string ServiceHistoryId { get; set; }

        [FirestoreProperty]
        private string AppointmentId { get; set; }

        [FirestoreProperty]
        private string Service { get; set; }

        [FirestoreProperty]
        private string PatientId { get; set; }

        [FirestoreProperty]
        private string NurseId { get; set; }

        [FirestoreProperty]
        private string DoctorId { get; set; }

        [FirestoreProperty]
        private DateTime ServiceDate { get; set; }

        [FirestoreProperty]
        private string Location { get; set; }

        [FirestoreProperty]
        private string ServiceOutcomes { get; set; }

        // private getters and setters
        private string getServiceHistoryId() => ServiceHistoryId;
        private string getAppointmentId() => AppointmentId;
        private string getService() => Service;
        private string getPatientId() => PatientId;
        private string getNurseId() => NurseId;
        private string getDoctorId() => DoctorId;
        private DateTime getServiceDate() => ServiceDate; 
        private string getLocation() => Location;
        private string getServiceOutcomes() => ServiceOutcomes;

        private void setServiceHistoryId(string serviceHistoryId) => ServiceHistoryId = serviceHistoryId;
        private void setAppointmentId(string appointmentId) => AppointmentId = appointmentId;
        private void setService(string service) => Service = service;
        private void setPatientId(string patientId) => PatientId = patientId;
        private void setNurseId(string nurseId) => NurseId = nurseId;
        private void setDoctorId(string doctorId) => DoctorId = doctorId;
        private void setServiceDate(DateTime serviceDate) => ServiceDate = serviceDate;
        private void setLocation(string location) => Location = location;
        private void setServiceOutcomes(string serviceOutcomes) => ServiceOutcomes = serviceOutcomes;
        
        // public getters and setters
        public Dictionary<string, object> getServiceHistoryDetails()
        {
            return new Dictionary<string, object>
            {
                { "ServiceHistoryId", getServiceHistoryId() },
                { "AppointmentId", getAppointmentId() },
                { "Service", getService() },
                { "PatientId", getPatientId() },
                { "NurseId", getNurseId() },
                { "DoctorId", getDoctorId() },
                { "ServiceDate", getServiceDate() },
                { "Location", getLocation() },
                { "ServiceOutcomes", getServiceOutcomes() }
            };
        }
        public static ServiceHistory setServiceHistoryDetails(string appointmentId, string service, string patientId, string nurseId, string doctorId, DateTime serviceDate, string location, string serviceOutcomes)
        {
            var serviceHistory = new ServiceHistory();

            serviceHistory.setAppointmentId(appointmentId);
            serviceHistory.setService(service);
            serviceHistory.setPatientId(patientId);
            serviceHistory.setNurseId(nurseId);
            serviceHistory.setDoctorId(doctorId);
            serviceHistory.setServiceDate(serviceDate);
            serviceHistory.setLocation(location);
            serviceHistory.setServiceOutcomes(serviceOutcomes);

            return serviceHistory;
        }

        public void updateServiceHistoryId(string serviceHistoryId) {
            setServiceHistoryId(serviceHistoryId);
        }
    }
}
