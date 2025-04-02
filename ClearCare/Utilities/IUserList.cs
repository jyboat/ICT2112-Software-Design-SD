namespace ClearCare
{
    public static class IUserList
    {
        /// <summary>
        ///   Example of the current user's UUID for testing purposes.
        /// </summary>
        public const string CurrentUserUUID = "uuid-alice";

        /// <summary>
        ///   Example of the current user's role for testing purposes.
        /// </summary>
        public const string CurrentUserRole = "Patient";

        /// <summary>
        ///   A simple user model for both patients and doctors.
        /// </summary>
        public class UserInfo
        {
            /// <summary>
            ///   Gets or sets the name of the user.
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            ///   Gets or sets the UUID of the user.
            /// </summary>
            public string Uuid { get; set; } = string.Empty;
        }

        /// <summary>
        ///   Gets the list of patients.
        /// </summary>
        public static List<UserInfo> Patients { get; } = new List<UserInfo>
        {
            new UserInfo { Name = "John Doe", Uuid = "uuid-patient-john" },
            new UserInfo { Name = "Sara", Uuid = "uuid-patient-sara" }
        };

        /// <summary>
        ///   Gets the list of doctors.
        /// </summary>
        public static List<UserInfo> Doctors { get; } = new List<UserInfo>
        {
            new UserInfo { Name = "Dr. John", Uuid = "uuid-doctor-john" }
        };
    }
}
