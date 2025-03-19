using ClearCare.Models.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace ClearCare.DataSource
{
    public class UserFactory
    {
        public static User createUser(string userID, string email, string password, string name, long mobileNumber, string address, string role, Dictionary<string, object> infoDictionary)
        {
            switch (role)
            {
                case "Doctor":
                    string specialization = (string)infoDictionary["Specialization"];
                    return new Doctor(userID, email, password, name, (int)mobileNumber, address, role, specialization);

                case "Nurse":
                    string department = (string)infoDictionary["Department"];
                    return new Nurse(userID, email, password, name, (int)mobileNumber, address, role, department);

                case "Admin":
                    string adminID = (string)infoDictionary["AdminID"];
                    string assignedBackupAdmin = (string)infoDictionary["AssignedBackupAdmin"];
                    return new Admin(userID, email, password, name, (int)mobileNumber, address, role, adminID, assignedBackupAdmin);

                case "Patient":
                    // Added null checks for Patient fields
                    string assignedCaregiverName = infoDictionary.ContainsKey("AssignedCaregiverName") ? 
                            (string)infoDictionary["AssignedCaregiverName"] : "";
                    string assignedCaregiverID = infoDictionary.ContainsKey("AssignedCaregiverID") ? 
                            (string)infoDictionary["AssignedCaregiverID"] : "";
                    Timestamp dateOfBirth;
                    if (infoDictionary.ContainsKey("DateOfBirth"))
                    {
                        if (infoDictionary["DateOfBirth"] is Timestamp timestamp)
                        {
                            dateOfBirth = timestamp;
                        }
                        else if (infoDictionary["DateOfBirth"] is string dateString)
                        {
                            // Try to parse the string to a DateTime
                            try
                            {
                                DateTime parsedDate = DateTime.Parse(dateString);
                                dateOfBirth = Timestamp.FromDateTime(DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc));
                            }
                            catch
                            {
                                // If parsing fails, use default value
                                dateOfBirth = Timestamp.FromDateTime(DateTime.UtcNow);
                            }
                        }
                        else
                        {
                            // Default for unexpected type
                            dateOfBirth = Timestamp.FromDateTime(DateTime.UtcNow);
                        }
                    }
                    else
                    {
                        // No DateOfBirth field
                        dateOfBirth = Timestamp.FromDateTime(DateTime.UtcNow);
                    }
                    return new Patient(userID, email, password, name, (int)mobileNumber, address, role, assignedCaregiverName, assignedCaregiverID, dateOfBirth);

                case "Caregiver":
                    string assignedPatientName = (string)infoDictionary["AssignedPatientName"];
                    string assignedPatientID = (string)infoDictionary["AssignedPatientID"];
                    return new Caregiver(userID, email, password, name, (int)mobileNumber, address, role, assignedPatientName, assignedPatientID);

                default:
                    throw new ArgumentException($"Invalid user role: {role}"); // Prevents creating abstract User
            }
        }
    }
}