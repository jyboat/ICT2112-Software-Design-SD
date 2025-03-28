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
            public string Name { get; set; }
            public string Uuid { get; set; }
        }

        // List of patients
        public static List<UserInfo> Patients { get; } = new List<UserInfo>
        {
            new UserInfo { Name = "John Doe",    Uuid = "uuid-patient-john" },
            new UserInfo { Name = "Jane Smith",  Uuid = "uuid-patient-jane" },
            new UserInfo { Name = "Bob Johnson", Uuid = "uuid-patient-bob" },
            new UserInfo { Name = "Alice",       Uuid = "uuid-alice" }
        };

        // List of doctors
        public static List<UserInfo> Doctors { get; } = new List<UserInfo>
        {
            new UserInfo { Name = "Dr. John Doe",  Uuid = "uuid-doctor-john" },
            new UserInfo { Name = "Dr. Jane Smith", Uuid = "uuid-doctor-jane" }
        };

        
    }
}
