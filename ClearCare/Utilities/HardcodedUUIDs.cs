namespace ClearCare
{
    public static class HardcodedUUIDs
    {
        // Doctore Exmaple
        // public const string UserUUID = "uuid-doctor-1"; 
        // public const string UserRole = "Doctor";

        // Paitent Exmaple
        public const string UserUUID = "uuid-alice";
        public const string UserRole = "Doctor";

        // Used for Prescription Page Drop Done
        public class PatientInfo
        {
            public string Name { get; set; }
            public string Uuid { get; set; }
        }

        // Hardcode a list of patients you can reference
        public static List<PatientInfo> Patients = new List<PatientInfo>
        {
            new PatientInfo { Name = "John Doe",    Uuid = "uuid-patient-john" },
            new PatientInfo { Name = "Jane Smith",  Uuid = "uuid-patient-jane" },
            new PatientInfo { Name = "Bob Johnson", Uuid = "uuid-patient-bob" },
            new PatientInfo { Name = "Alice", Uuid = "uuid-alice" }

            // Add as many as you want
        };
        
    }
}
