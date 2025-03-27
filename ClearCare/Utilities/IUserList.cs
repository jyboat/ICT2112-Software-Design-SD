using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare
{
    // Minimal User class for demonstration
    public class User
    {
        public string Name { get; set; }
        public string Uuid { get; set; }
        public string Role { get; set; }
    }

    // Interface specifying methods to retrieve user details by role
    public interface IUserDetails
    {
        Task<List<User>> getAllPatients();
        Task<List<User>> getAllDoctors();
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

        // Returns a list of all patients (users with Role "Patient")
        public Task<List<User>> getAllPatients()
        {
            var patientsList = Patients
                .Where(p => p.Role.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                .Select(p => new User
                {
                    Name = p.Name,
                    Uuid = p.Uuid,
                    Role = p.Role
                }).ToList();

            return Task.FromResult(patientsList);
        }

        // Returns a list of all doctors (users with Role "Doctor")
        public Task<List<User>> getAllDoctors()
        {
            var doctorsList = Patients
                .Where(p => p.Role.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
                .Select(p => new User
                {
                    Name = p.Name,
                    Uuid = p.Uuid,
                    Role = p.Role
                }).ToList();

            return Task.FromResult(doctorsList);
        }
    }
}
