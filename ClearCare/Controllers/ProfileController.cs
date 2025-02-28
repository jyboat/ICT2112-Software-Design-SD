using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    [Route("ViewProfile")] // Set base route for the controller
    public class ProfileController : Controller
    {
        private readonly ProfileManagement _profileManagement; //readonly to prevent unintended changes in other methods

        public ProfileController()
        {
            _profileManagement = new ProfileManagement();
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
                return RedirectToAction("Login", "Auth");
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
        public async Task<IActionResult> UpdateProfile([FromForm] string name, [FromForm] string email, [FromForm] long mobileNumber, [FromForm] string address, [FromForm] string password)
        {
            // Fetch logged-in user ID from session
            string userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Prepare updated fields dictionary
            Dictionary<string, object> updatedFields = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(name)) updatedFields["Name"] = name;
            if (!string.IsNullOrEmpty(email)) updatedFields["Email"] = email;
            if (mobileNumber > 0) updatedFields["MobileNumber"] = mobileNumber;
            if (!string.IsNullOrEmpty(address)) updatedFields["Address"] = address;
            if (!string.IsNullOrEmpty(password)) updatedFields["Password"] = password;

            // Call the update function
            bool result = await _profileManagement.UpdateUserProfile(userID, updatedFields);

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
