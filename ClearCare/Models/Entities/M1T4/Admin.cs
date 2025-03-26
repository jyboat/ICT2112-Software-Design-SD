using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace ClearCare.Models.Entities
{
     [FirestoreData]
     public class Admin : User
     {
          [FirestoreProperty]
          protected string AdminID { get; set; }

          [FirestoreProperty]
          protected string AssignedBackupAdmin { get; set; }

          protected string getAdminID() => AdminID;
          protected string getAssignedBackupAdmin() => AssignedBackupAdmin;

          protected void setAdminID(string adminID) => AdminID = adminID;
          protected void setAssignedBackupAdmin(string assignedBackupAdmin) => AssignedBackupAdmin = assignedBackupAdmin;

          public Admin() { }

          public Admin(string userID, string email, string password, string name, long mobileNumber, string address, string role,
                           string adminID, string assignedBackupAdmin)
              : base(userID, email, password, name, mobileNumber, address, role)  // Call base constructor
          {
               AdminID = adminID;
               AssignedBackupAdmin = assignedBackupAdmin;
          }

          // Override GetProfileData() to include Admin-specific fields
          public override Dictionary<string, object> getProfileData()
          {
               var details = base.getProfileData();
               details.Add("AdminID", getAdminID());
               details.Add("AssignedBackupAdmin", getAssignedBackupAdmin());
               return details;
          }
     }
}
