using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
     public class AdminController : Controller
     {
          private readonly AdminAccountManagement _adminAccountManagement;

          public AdminController()
          {
               var userGateway = new UserGateway();
               _adminAccountManagement = new AdminAccountManagement(userGateway);
          }

          // GET: /Admin/LoadCreateNurseAccount
          [HttpGet]
          public IActionResult LoadCreateNurseAccount()
          {
               return View("~/Views/Admin/CreateNurseAccount.cshtml");
          }

          // GET: /Admin/LoadCreateDoctorAccount
          [HttpGet]
          public IActionResult LoadCreateDoctorAccount()
          {
               return View("~/Views/Admin/CreateDoctorAccount.cshtml");
          }

          // POST: /Admin/CreateAccount
          [HttpPost]
          public async Task<IActionResult> CreateAccount(string email, string password, string name, string department, string specialization, long mobileNumber, string address, string role)
          {
               if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
               {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
                    return View("~/Views/Admin/CreateAccount.cshtml");
               }

               Console.WriteLine(department);

               return View("~/Views/Admin/CreateAccount.cshtml");
          }
     }
}