using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Globalization;

namespace ClearCare.Controllers
{
    [Route("ViewProfile")] // Set base route for the controller
    public class ProfileController : Controller
    {
        private readonly ProfileManagement _profileManagement; //readonly to prevent unintended changes in other methods
        private readonly EncryptionManagement encryptionManagement;
        private readonly DelegateManagement _delegateManagement;

        public ProfileController()
        {
            _profileManagement = new ProfileManagement();
            encryptionManagement = new EncryptionManagement();
            _delegateManagement = new DelegateManagement();
        }

        // Display user profile (Now accessible via /ViewProfile)
        [HttpGet] // Only allowing GET
        [Route("")]
        public async Task<IActionResult> displayProfile()
        {
            // Fetch the logged-in user's ID from session
            string userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayLogin", "Login");
            }

            // Fetch user details through ProfileManagement (Control Class)
            User user = await _profileManagement.getUserDetails(userID);

            if (user == null)
            {
                return View("Error", "User not found.");
            }

            return View("ViewProfile", user); // ViewModel
        }

        // Display Update Profile View
        [HttpGet]
        [Route("UpdateProfile")]
        public async Task<IActionResult> displayUpdateProfile()
        {
            // Fetch the logged-in user's ID from session
            string userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayProfile"); // Redirect if user is null
            }

            // Fetch user details through ProfileManagement (Control Class)
            User user = await _profileManagement.getUserDetails(userID);

            if (user == null)
            {
                return View("Error", "User not found.");
            }

            return View("UpdateProfile", user); // Correct ViewModel
        }


        // Update user profile (POST request)
        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<IActionResult> updateProfile([FromForm] string name, [FromForm] string email, [FromForm] long mobileNumber, [FromForm] string address, [FromForm] string newpassword,[FromForm] string confirmpassword , [FromForm] string dateOfBirth, [FromForm] long countryCode)
        {
            // Fetch logged-in user ID from session
            string userID = HttpContext.Session.GetString("UserID");

            // Ensure UserID exists
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayLogin", "Login");
            }

            // Prepare updated fields dictionary
            Dictionary<string, object> updatedFields = new Dictionary<string, object>();

            // Name
            if (!string.IsNullOrEmpty(name)) updatedFields["Name"] = name;
            
            // Email Validation (Cant use email that exists)
            if (!string.IsNullOrEmpty(email))
            {
                bool emailExists = await _profileManagement.emailExists(email, userID); // Exclude current user
                if (emailExists)
                {
                    TempData["ErrorMessage"] = "Email is already in use.";
                    return RedirectToAction("displayProfile");
                }
                else
                {
                    updatedFields["Email"] = email; // debugged code
                }
            }

            // Mobile Number Validation (Cant use mobile number that exists)
            if (mobileNumber > 0)
            {
                bool mobileExists = await _profileManagement.mobileExists(mobileNumber, userID); // Exclude current user
                if (mobileExists)
                {
                    TempData["ErrorMessage"] = "Mobile number is already in use.";
                    return RedirectToAction("displayProfile");
                }
                else
                {
                    // Combine country code and mobile number NEW CODE ADDED for country code
                    string mobileStr = mobileNumber.ToString();
                    string fullMobileNumber = countryCode.ToString() + mobileStr;
                    updatedFields["MobileNumber"] = long.Parse(fullMobileNumber);
                }
            }

            // Address
            if (!string.IsNullOrEmpty(address)) updatedFields["Address"] = address;

            // Validate password match
            if (!string.IsNullOrEmpty(newpassword) && !string.IsNullOrEmpty(confirmpassword))
            {
                if (newpassword != confirmpassword)
                {
                    TempData["ErrorMessage"] = "Passwords do not match.";
                    return RedirectToAction("displayProfile");
                }
                    
                // Hash the password before updating it in Firestore
                string hashedPassword = encryptionManagement.hashPassword(confirmpassword);
                updatedFields["Password"] = hashedPassword;
            }

            // Date Of Birth Validation
            if (!string.IsNullOrEmpty(dateOfBirth))
            {
                try
                {
                    // Parse user input from "datetime-local" (which is in UTC+8)
                    if (DateTime.TryParseExact(dateOfBirth, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDob))
                    {
                        // Convert from UTC+8 to UTC before storing in Firestore
                        DateTime dobUtc = parsedDob.AddHours(-8); // Subtract 8 hours to store in UTC

                        updatedFields["DateOfBirth"] = Timestamp.FromDateTime(DateTime.SpecifyKind(dobUtc, DateTimeKind.Utc));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid date format. Please enter a valid date and time.";
                        return RedirectToAction("displayProfile");
                    }
                }
                catch
                {
                    TempData["ErrorMessage"] = "Error processing date of birth.";
                    return RedirectToAction("displayProfile");
                }
            }

            // Call the update function
            bool result = await _profileManagement.editUserDetails(userID, updatedFields);

            if (result)
            {
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("displayProfile");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update profile. Please try again.";
                return RedirectToAction("displayProfile");
            }
        }

        // Display Assign Caregiver View
        [HttpGet]
        [Route("AssignCaregiver")]
        public async Task<IActionResult> displayAssignCaregiver()
        {
            // Fetch the logged-in user's ID from session
            string userID = HttpContext.Session.GetString("UserID");
            
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayLogin", "Login");
            }

            // For debugging - temporarily display all caregivers regardless of role
            // Note: In production, you would want to restore the role check
            List<Caregiver> availableCaregivers = await _delegateManagement.getAllCaregivers();

            if (availableCaregivers == null || availableCaregivers.Count == 0)
            {
                ViewBag.CaregiverInfo = "No caregivers found in the system.";
            }
            else
            {
                ViewBag.CaregiverInfo = $"Found {availableCaregivers.Count} caregivers.";
            }

            return View("Delegate", availableCaregivers);
        }

        // Display Remove Caregiver View
        [HttpGet]
        [Route("RemoveCaregiverView")]
        public async Task<IActionResult> displayRemoveCaregiver()
        {
            // Fetch the logged-in user's ID from session
            string userID = HttpContext.Session.GetString("UserID");
            
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayLogin", "Login");
            }

            // Fetch user details
            User user = await _profileManagement.getUserDetails(userID);
            
            if (user == null)
            {
                return View("Error", "User not found.");
            }

            // Cast to patient
            Patient patient = user as Patient;
            
            if (patient == null)
            {
                // If not a patient, display an error
                TempData["ErrorMessage"] = "This feature is only available for patients.";
                return RedirectToAction("displayProfile");
            }

            return View("RemoveCaregiver", patient);
        }

        // Process Assign Caregiver
        [HttpPost]
        [Route("Delegate")]
        public async Task<IActionResult> Delegate([FromForm] string caregiverID)
        {
            // Fetch the logged-in user's ID from session
            string userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayLogin", "Login");
            }

            if (string.IsNullOrEmpty(caregiverID))
            {
                TempData["ErrorMessage"] = "Please select a caregiver.";
                return RedirectToAction("displayAssignCaregiver");
            }

            // Assign the caregiver to the patient
            string result = await _delegateManagement.assignCaregiver(userID, caregiverID);

            if (!string.IsNullOrEmpty(result))
            {
                TempData["SuccessMessage"] = "Caregiver assigned successfully.";
                return RedirectToAction("displayProfile");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to assign caregiver. Please try again.";
                return RedirectToAction("displayAssignCaregiver");
            }
        }

        // Process Remove Caregiver
        [HttpPost]
        [Route("RemoveCaregiver")]
        public async Task<IActionResult> RemoveCaregiver()
        {
            // Fetch the logged-in user's ID from session
            string userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayLogin", "Login");
            }

            // Remove the caregiver from the patient
            string result = await _delegateManagement.removeCaregiver(userID);

            if (!string.IsNullOrEmpty(result))
            {
                TempData["SuccessMessage"] = "Caregiver removed successfully.";
                return RedirectToAction("displayProfile");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to remove caregiver. Please try again.";
                return RedirectToAction("displayRemoveCaregiver");
            }
        }
    }
}