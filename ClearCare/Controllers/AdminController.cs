using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

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

          // GET: /Admin/Dashboard
          [HttpGet]
          public async Task<IActionResult> Dashboard()
          {
               var users = await _adminAccountManagement.RetrieveAllUsers();

               if (users != null)
               {
                    var sortedUsers = users.OrderBy(u => u.GetUserID()).ToList();
                    ViewData["Users"] = sortedUsers;
               }

               return View("~/Views/Admin/Dashboard.cshtml");
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

          public IActionResult LoadRoleForm(string role)
          {
               if (role == "Nurse")
               {
                    return View("~/Views/Admin/CreateNurseAccount.cshtml");
               }
               if (role == "Doctor")
               {
                    return View("~/Views/Admin/CreateDoctorAccount.cshtml");
               }
               else
               {
                    // Handle unexpected roles
                    return NotFound("Invalid role specified.");
               }
          }

          // POST: /Admin/CreateAccount
          [HttpPost]
          public async Task<IActionResult> CreateAccount(string email, string password, string name, string role, string address, long? mobileNumber, string? department, string? specialization)
          {
               if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || mobileNumber == null || (string.IsNullOrEmpty(department) && string.IsNullOrEmpty(specialization)))
               {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
                    return RedirectToAction("LoadRoleForm", new { role = role });
               }

               User newUser = null;

               if (role == "Nurse" && !string.IsNullOrEmpty(department))
               {
                    newUser = new Nurse("", email, password, name, (long)mobileNumber, address, role, department);
               }
               else if (role == "Doctor" && !string.IsNullOrEmpty(specialization))
               {
                    newUser = new Doctor("", email, password, name, (long)mobileNumber, address, role, specialization);
               }

               string result = await _adminAccountManagement.CreateStaffAccount(newUser, password);

               if (result == "Account created successfully.")
               {
                    ViewBag.SuccessMessage = result;
                    return RedirectToAction("Index", "Home");
               }
               else
               {
                    ViewBag.ErrorMessage = result;
                    return RedirectToAction("LoadRoleForm", new { role = role });
               }
          }
     }
}