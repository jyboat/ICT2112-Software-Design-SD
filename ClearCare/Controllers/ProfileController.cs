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
    }
}
