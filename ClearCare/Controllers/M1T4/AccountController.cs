using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface;

namespace ClearCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountManagement _accountManagement;

        private readonly AuditManagement _auditManagement; // ✅ Declare AuditManagement

        public AccountController(IPassword passwordService)
    {
        var userGateway = new UserGateway(); // ✅ Declare userGateway
        _accountManagement = new AccountManagement(userGateway, passwordService); // ✅ Pass passwordService
        _auditManagement = new AuditManagement(); // ✅ Initialize AuditManagement
    }
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Register/Register.cshtml");
        }


        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string name, long mobileNumber, string address, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
            {
                ViewBag.ErrorMessage = "Please fill in all required fields.";
                return View("~/Views/Register/Register.cshtml");
            }

            Dictionary<string, object> infoDictionary = new Dictionary<string, object>();

            if (role == "Patient")
            {
                infoDictionary.Add("AssignedCaregiverName", "");
                infoDictionary.Add("AssignedCaregiverID", "");
                infoDictionary.Add("DateOfBirth", Timestamp.FromDateTime(DateTime.UtcNow));
            }
            else if (role == "Caregiver")
            {
                infoDictionary.Add("AssignedPatientName", "");
                infoDictionary.Add("AssignedPatientID", "");
            }

            User newUser = UserFactory.createUser("", email, password, name, (int)mobileNumber, address, role, infoDictionary);
            string result = await _accountManagement.CreateAccount(newUser, password, _auditManagement);

            if (result == "Account created successfully.")
            {
                ViewBag.SuccessMessage = result;
                return RedirectToAction("DisplayLogin", "Login");
                TempData["SuccessMessage"] = "Your message";
            }
            else
            {
                ViewBag.ErrorMessage = result;
                return View("~/Views/Register/Register.cshtml");
            }
        }
    }
}
