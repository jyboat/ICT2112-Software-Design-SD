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

        public ProfileController()
        {
            _profileManagement = new ProfileManagement();
            encryptionManagement = new EncryptionManagement();
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
        public async Task<IActionResult> updateProfile([FromForm] string name, [FromForm] string email, [FromForm] long mobileNumber, [FromForm] string address, [FromForm] string password, [FromForm] string dateOfBirth)
        {
            // Fetch logged-in user ID from session
            string userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("displayLogin", "Login");
            }

            // Prepare updated fields dictionary
            Dictionary<string, object> updatedFields = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(name)) updatedFields["Name"] = name;
            if (!string.IsNullOrEmpty(email)) updatedFields["Email"] = email;
            if (mobileNumber > 0) updatedFields["MobileNumber"] = mobileNumber;
            if (!string.IsNullOrEmpty(address)) updatedFields["Address"] = address;
            if (!string.IsNullOrEmpty(password))
            {
                // Hash the password before updating it in Firestore
                string hashedPassword = encryptionManagement.hashPassword(password);
                updatedFields["Password"] = hashedPassword;
            }
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
    }
}
