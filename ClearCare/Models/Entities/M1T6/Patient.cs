using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class Patient : User
    {
        // Class properties
        [FirestoreProperty]
        protected string AssignedCaregiverName { get; set; }

        [FirestoreProperty]
        protected string AssignedCaregiverID { get; set; }

        [FirestoreProperty]
        protected Timestamp DateOfBirth { get; set; }  // Firestore Timestamp

        // Getter & Setter
        protected string getAssignedCaregiverName() => AssignedCaregiverName;
        protected string getAssignedCaregiverID() => AssignedCaregiverID;
        protected Timestamp  getDateOfBirth() => DateOfBirth;

        protected void setAssignedCaregiverName(string caregiverName) => AssignedCaregiverName = caregiverName;
        protected void setAssignedCaregiverID(string caregiverID) => AssignedCaregiverID = caregiverID;
        protected void setDateOfBirth(Timestamp  dob) => DateOfBirth = dob;

        public Patient() {}

        // Object Creation
        public Patient(string userID, string email, string password, string name, long mobileNumber, string address, string role,
                       string assignedCaregiverName, string assignedCaregiverID, Timestamp  dateOfBirth)
            : base(userID, email, password, name, mobileNumber, address, role)  // Call base constructor
        {
            AssignedCaregiverName = assignedCaregiverName;
            AssignedCaregiverID = assignedCaregiverID;
            DateOfBirth = dateOfBirth;
        }

        public Patient(string userID, string email, string password, string name, long mobileNumber, string address, string role)
            : base(userID, email, password, name, mobileNumber, address, role)  // Call base constructor
        {
            AssignedCaregiverName = "";
            AssignedCaregiverID = "";
            DateOfBirth = Timestamp.FromDateTime(DateTime.UtcNow);
        }

        // Override GetProfileData() to include Patient-specific fields
        public override Dictionary<string, object> getProfileData()
        {
            var details = base.getProfileData();
            details.Add("AssignedCaregiverName", getAssignedCaregiverName());
            details.Add("AssignedCaregiverID", getAssignedCaregiverID());
            // details.Add("DateOfBirth", GetDateOfBirth().ToDateTime().ToString("yyyy-MM-dd"));  // Convert to readable format
            // Convert UTC to UTC+8 manually
            var dobUtc = DateTime.SpecifyKind(getDateOfBirth().ToDateTime(), DateTimeKind.Utc);
            var dobUtcPlus8 = dobUtc.AddHours(8); // Convert UTC to UTC+8

            details.Add("DateOfBirth", dobUtcPlus8.ToString("dd MMMM yyyy HH:mm:ss"));
            return details;
        }
    }
}
