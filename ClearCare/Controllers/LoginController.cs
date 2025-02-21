using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginManagement LoginManagement;

        public LoginController()
        {
            LoginManagement = new LoginManagement();
        }

        public IActionResult DisplayLogin()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(string email, string password)
        {
            var authenticatedUser = await LoginManagement.AuthenticateUser(email, password);
            if (authenticatedUser != null)
            {
                // Store only UserID & Role in session
                var (userID, role) = authenticatedUser.GetSessionData();
                HttpContext.Session.SetString("UserID", userID);
                HttpContext.Session.SetString("Role", role);

                // redirect to Homepage
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Invalid login credentials";
            return View("Login");
        }
    }
}