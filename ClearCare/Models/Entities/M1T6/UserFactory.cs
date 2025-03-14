using ClearCare.Models.Entities;
using Google.Cloud.Firestore;

namespace ClearCare.DataSource
{
    public class UserFactory
    {
        public static User createUser(string userID, string email, string password, string name, long mobileNumber, string address, string role, DocumentSnapshot snapshot)
        {
            switch (role)
            {
                case "Doctor":
                    string specialization = snapshot.GetValue<string>("Specialization");
                    return new Doctor(userID, email, password, name, (int)mobileNumber, address, role, specialization);

                case "Nurse":
                    string department = snapshot.GetValue<string>("Department");
                    return new Nurse(userID, email, password, name, (int)mobileNumber, address, role, department);

                case "Patient":
                    string assignedCaregiverName = snapshot.GetValue<string>("AssignedCaregiverName");
                    string assignedCaregiverID = snapshot.GetValue<string>("AssignedCaregiverID");
                    Timestamp dateOfBirth = snapshot.GetValue<Timestamp>("D`ateOfBirth");
                    return new Patient(userID, email, password, name, (int)mobileNumber, address, role, assignedCaregiverName, assignedCaregiverID, dateOfBirth);

                case "Caregiver":
                    string assignedPatientName = snapshot.GetValue<string>("AssignedPatientName");
                    string assignedPatientID = snapshot.GetValue<string>("AssignedPatientID");
                    return new Caregiver(userID, email, password, name, (int)mobileNumber, address, role, assignedPatientName, assignedPatientID);

                default:
                    throw new ArgumentException($"Invalid user role: {role}"); // Prevents creating abstract User
            }
        }
    }
}
