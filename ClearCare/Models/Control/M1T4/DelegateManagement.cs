using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models.Entities;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class DelegateManagement
    {
        private readonly UserGateway _userGateway;
        
        public DelegateManagement()
        {
            _userGateway = new UserGateway();
        }

        // Method to assign a caregiver to a patient
        public async Task<string> assignCaregiver(string patientID, string caregiverID)
        {
            try
            {
                // First, get the patient and caregiver details
                Patient patient = await _userGateway.findUserByID(patientID) as Patient;
                Caregiver caregiver = await _userGateway.findUserByID(caregiverID) as Caregiver;
                
                if (patient == null || caregiver == null)
                {
                    Console.WriteLine("Patient or Caregiver not found");
                    return null;
                }
                
                // Get names for reference using getProfileData()
                var patientData = patient.getProfileData();
                var caregiverData = caregiver.getProfileData();
                
                string patientName = patientData["Name"].ToString();
                string caregiverName = caregiverData["Name"].ToString();
                
                // Prepare update for patient
                Dictionary<string, object> patientUpdate = new Dictionary<string, object>
                {
                    { "AssignedCaregiverID", caregiverID },
                    { "AssignedCaregiverName", caregiverName }
                };
                
                // Prepare update for caregiver
                Dictionary<string, object> caregiverUpdate = new Dictionary<string, object>
                {
                    { "AssignedPatientID", patientID },
                    { "AssignedPatientName", patientName }
                };
                
                // Update both records
                bool patientUpdateSuccess = await _userGateway.updateUser(patientID, patientUpdate);
                bool caregiverUpdateSuccess = await _userGateway.updateUser(caregiverID, caregiverUpdate);
                
                if (patientUpdateSuccess && caregiverUpdateSuccess)
                {
                    return caregiverID; // Return the assigned caregiver's ID on success
                }
                else
                {
                    Console.WriteLine("Failed to update patient or caregiver records");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in assignCaregiver: {ex.Message}");
                return null;
            }
        }
        
        // Method to remove a caregiver from a patient
        public async Task<string> removeCaregiver(string patientID)
        {
            try
            {
                // First, get the patient details
                Patient patient = await _userGateway.findUserByID(patientID) as Patient;
                
                if (patient == null)
                {
                    Console.WriteLine("Patient not found");
                    return null;
                }
                
                // Get the current caregiver ID before clearing it
                var patientData = patient.getProfileData();
                string currentCaregiverID = patientData.ContainsKey("AssignedCaregiverID") ? 
                                           patientData["AssignedCaregiverID"].ToString() : "";
                
                if (string.IsNullOrEmpty(currentCaregiverID))
                {
                    Console.WriteLine("No caregiver assigned to patient");
                    return null;
                }
                
                // Get the caregiver to also update their record
                Caregiver caregiver = await _userGateway.findUserByID(currentCaregiverID) as Caregiver;
                
                // Prepare update for patient - clear caregiver info
                Dictionary<string, object> patientUpdate = new Dictionary<string, object>
                {
                    { "AssignedCaregiverID", "" },
                    { "AssignedCaregiverName", "" }
                };
                
                // Update patient record
                bool patientUpdateSuccess = await _userGateway.updateUser(patientID, patientUpdate);
                
                // If caregiver exists, update their record too
                bool caregiverUpdateSuccess = true;
                if (caregiver != null)
                {
                    Dictionary<string, object> caregiverUpdate = new Dictionary<string, object>
                    {
                        { "AssignedPatientID", "" },
                        { "AssignedPatientName", "" }
                    };
                    
                    caregiverUpdateSuccess = await _userGateway.updateUser(currentCaregiverID, caregiverUpdate);
                }
                
                if (patientUpdateSuccess && caregiverUpdateSuccess)
                {
                    return currentCaregiverID; // Return the removed caregiver's ID on success
                }
                else
                {
                    Console.WriteLine("Failed to update patient or caregiver records");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in removeCaregiver: {ex.Message}");
                return null;
            }
        }
        
        // Method to get all available caregivers (those not assigned to any patient)
        public async Task<List<Caregiver>> getAvailableCaregivers()
        {
            try
            {
                List<Caregiver> availableCaregivers = new List<Caregiver>();
                List<User> allUsers = await _userGateway.getAllUsers();
                
                foreach (User user in allUsers)
                {
                    if (user is Caregiver caregiver)
                    {
                        // Check if the caregiver is not assigned to any patient
                        var caregiverData = caregiver.getProfileData();
                        string assignedPatientID = caregiverData.ContainsKey("AssignedPatientID") ? 
                                                  caregiverData["AssignedPatientID"].ToString() : "";
                        
                        if (string.IsNullOrEmpty(assignedPatientID))
                        {
                            availableCaregivers.Add(caregiver);
                        }
                    }
                }
                
                return availableCaregivers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getAvailableCaregivers: {ex.Message}");
                return new List<Caregiver>();
            }
        }
        
        // Method to get all caregivers
        public async Task<List<Caregiver>> getAllCaregivers()
        {
            try
            {
                List<Caregiver> caregivers = new List<Caregiver>();
                List<User> allUsers = await _userGateway.getAllUsers();
                
                foreach (User user in allUsers)
                {
                    if (user is Caregiver caregiver)
                    {
                        caregivers.Add(caregiver);
                    }
                }
                
                return caregivers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getAllCaregivers: {ex.Message}");
                return new List<Caregiver>();
            }
        }
    }
}