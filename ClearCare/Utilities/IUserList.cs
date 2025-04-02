namespace ClearCare
{
    public static class IUserList
    {
        // Example of current user details for testing purposes
        public const string CurrentUserUUID = "uuid-alice";
        public const string CurrentUserRole = "Patient";
        
        // A simple user model for both patients and doctors.
        public class UserInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Uuid { get; set; } = string.Empty;
        }

        // List of patients
        public static List<UserInfo> Patients { get; } = new List<UserInfo>
        {
            new UserInfo { Name = "John Doe",    Uuid = "uuid-patient-john" },
            new UserInfo { Name = "Sara",       Uuid = "uuid-patient-sara" }

        };

        // List of doctors
        public static List<UserInfo> Doctors { get; } = new List<UserInfo>
        {
            new UserInfo { Name = "Dr. John",  Uuid = "uuid-doctor-john" },
        };

        
    }
}
