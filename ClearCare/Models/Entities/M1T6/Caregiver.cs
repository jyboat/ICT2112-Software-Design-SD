using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace ClearCare.Models.Entities
{
    [FirestoreData]
    public class Caregiver : User
    {
        [FirestoreProperty]
        protected string AssignedPatientName { get; set; }

        [FirestoreProperty]
        protected string AssignedPatientID { get; set; }

        protected string getAssignedPatientName() => AssignedPatientName;
        protected string getAssignedPatientID() => AssignedPatientID;

        protected void setAssignedPatientName(string patientName) => AssignedPatientName = patientName;
        protected void setAssignedPatientID(string patientID) => AssignedPatientID = patientID;

        public Caregiver() {}

        public Caregiver(string userID, string email, string password, string name, long mobileNumber, string address, string role,
                         string assignedPatientName, string assignedPatientID)
            : base(userID, email, password, name, mobileNumber, address, role)  // Call base constructor
        {
            AssignedPatientName = assignedPatientName;
            AssignedPatientID = assignedPatientID;
        }

        // Override GetProfileData() to include Caregiver-specific fields
        public override Dictionary<string, object> getProfileData()
        {
            var details = base.getProfileData();
            details.Add("AssignedPatientName", getAssignedPatientName());
            details.Add("AssignedPatientID", getAssignedPatientID());
            return details;
        }
    }
}
