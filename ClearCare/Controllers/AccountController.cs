using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountManagement _accountManagement;

        public AccountController()
        {
            var userGateway = new UserGateway();
            _accountManagement = new AccountManagement(userGateway);
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

            string result = await _accountManagement.CreateAccount(email, password, name, mobileNumber, address, role);

            if (result == "Account created successfully.")
            {
                ViewBag.SuccessMessage = result;
                return RedirectToAction("DisplayLogin", "Login");
            }
            else
            {
                ViewBag.ErrorMessage = result;
                return View("~/Views/Register/Register.cshtml");
            }
        }
    }
}
