using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.Details
{
    // Minimal User class for demonstration
    public class User
    {
        public string Name { get; set; }
        public string Uuid { get; set; }
        public string Role { get; set; }
    }

    // Interface specifying the method to retrieve a single user's details by UUID
    public interface IUserDetails
    {
        Task<User> getUserDetails(string uuid);
    }

    // Implementation of IUserDetails
    public class UserDetails : IUserDetails
    {
        // Internal model to hold each user's info, including role
        public class PatientInfo
        {
            public string Name { get; set; }
            public string Uuid { get; set; }
            public string Role { get; set; }
        }

        // Hardcoded list with 1 doctor and 1 patient
        private static readonly List<PatientInfo> Patients = new List<PatientInfo>
        {
            new PatientInfo { Name = "Dr. John Doe", Uuid = "uuid-doctor-john", Role = "Doctor" },
            new PatientInfo { Name = "Alice",       Uuid = "uuid-patient-alice", Role = "Patient" }
        };

        // Returns the user's details based on the provided UUID.
        // If no user is found, this returns null.
        public Task<User> getUserDetails(string uuid)
        {
            var patient = Patients.FirstOrDefault(p => p.Uuid == uuid);
            if (patient == null)
            {
                return Task.FromResult<User>(null);
            }

            var user = new User
            {
                Name = patient.Name,
                Uuid = patient.Uuid,
                Role = patient.Role
            };

            return Task.FromResult(user);
        }
    }
}
