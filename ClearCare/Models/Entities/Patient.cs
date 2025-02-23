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
        protected string GetAssignedCaregiverName() => AssignedCaregiverName;
        protected string GetAssignedCaregiverID() => AssignedCaregiverID;
        protected Timestamp  GetDateOfBirth() => DateOfBirth;

        protected void SetAssignedCaregiverName(string caregiverName) => AssignedCaregiverName = caregiverName;
        protected void SetAssignedCaregiverID(string caregiverID) => AssignedCaregiverID = caregiverID;
        protected void SetDateOfBirth(Timestamp  dob) => DateOfBirth = dob;

        // Object Creation
        public Patient(string userID, string email, string password, string name, long mobileNumber, string address, string role,
                       string assignedCaregiverName, string assignedCaregiverID, Timestamp  dateOfBirth)
            : base(userID, email, password, name, mobileNumber, address, role)  // Call base constructor
        {
            AssignedCaregiverName = assignedCaregiverName;
            AssignedCaregiverID = assignedCaregiverID;
            DateOfBirth = dateOfBirth;
        }

        // Override GetProfileData() to include Patient-specific fields
        public override Dictionary<string, object> GetProfileData()
        {
            var details = base.GetProfileData();
            details.Add("AssignedCaregiverName", GetAssignedCaregiverName());
            details.Add("AssignedCaregiverID", GetAssignedCaregiverID());
            details.Add("DateOfBirth", GetDateOfBirth().ToDateTime().ToString("yyyy-MM-dd"));  // Convert to readable format
            return details;
        }
    }
}
