using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Data;
using System.Reflection;
using ClearCare.Business;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
     public class AdminController : Controller
     {
          private readonly AdminManagement _adminManagement;
          private readonly AuditManagement _auditManagement; // Add AuditManagement

          public AdminController()
          {
               var userGateway = new UserGateway();
               _adminManagement = new AdminManagement(userGateway);
               _auditManagement = new AuditManagement(); // Initialize AuditManagement
          }

          // GET: /Admin/Dashboard
          [HttpGet]
          public async Task<IActionResult> Dashboard(string currentView = "medical")
          {
               var users = await _adminManagement.retrieveAllUsers();

               if (users != null)
               {
                    List<User> filteredUsers;

                    if (currentView == "medical") // for doctors and nurses
                    {
                         // Filter for doctors and nurses
                         filteredUsers = users
                              .Where(u => new[] { "Doctor", "Nurse" }.Contains(u.getProfileData()["Role"]))
                              .OrderBy(u => u.getProfileData()["UserID"])
                              .ToList();
                    }
                    else // for patients and caregiver
                    {
                         // Filter for doctors and nurses
                         filteredUsers = users
                              .Where(u => new[] { "Patient", "Caregiver" }.Contains(u.getProfileData()["Role"]))
                              .OrderBy(u => u.getProfileData()["UserID"])
                              .ToList();
                    }

                    ViewData["Users"] = filteredUsers;
                    ViewData["CurrentView"] = currentView;
               }

               return View("~/Views/Admin/Dashboard.cshtml");
          }

          // ✅ NEW: GET: /Admin/AuditView
          [HttpGet]
          public async Task<IActionResult> AuditView()
          {
               List<AuditLog> auditLogs = await _auditManagement.GetAllAuditLogsAsync();

               // Ensure ViewData is not null
               ViewData["AuditLogs"] = auditLogs ?? new List<AuditLog>();

               return View("~/Views/Admin/AuditView.cshtml");
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
                    return View("~/Views/Admin/CreatePatientCaregiverAccount.cshtml");
               }
          }

          // POST: /Admin/createAccount
          [HttpPost]
          public async Task<IActionResult> createAccount(string email, string password, string name, string role, string address, long? mobileNumber, string? department, string? specialization)
          {
               if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || mobileNumber == null || (string.IsNullOrEmpty(department) && string.IsNullOrEmpty(specialization)))
               {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
                    return RedirectToAction("LoadRoleForm", new { role = role });
               }

               Dictionary<string, object> infoDictionary = new Dictionary<string, object>();

               if (role == "Nurse" && !string.IsNullOrEmpty(department))
               {
                    infoDictionary.Add("Department", department);
               }
               else if (role == "Doctor" && !string.IsNullOrEmpty(specialization))
               {
                    infoDictionary.Add("Specialization", specialization);
               }

               User newUser = UserFactory.createUser("", email, password, name, (int)mobileNumber, address, role, infoDictionary);

               string result = await _adminManagement.createAccount(newUser!, password);

               if (result == "Account created successfully.")
               {
                    ViewBag.SuccessMessage = result;
                    return RedirectToAction("Dashboard");
               }
               else
               {
                    ViewBag.ErrorMessage = result;
                    return RedirectToAction("LoadRoleForm", new { role });
               }
          }

          // GET: /Admin/UpdateProfilePage/{uid}
          [HttpGet("/UpdateProfilePage/{uid}")]
          public async Task<IActionResult> manageUsers(string uid)
          {
               if (string.IsNullOrEmpty(uid))
               {
                    TempData["ErrorMessage"] = "User ID is required.";
                    return RedirectToAction("Dashboard"); // Redirect to Dashboard or another page
                    return RedirectToAction("Dashboard");
               }

               // Get user details from Firestore
               var user = await _adminManagement.retrieveUserByID(uid);

               // load the user data into the view
               ViewData["User"] = user;

               // load the correct view
               if (user.getProfileData()["Role"].ToString() == "Nurse")
               {
                    return View("~/Views/Admin/UpdateNurseAccount.cshtml");
               }
               else if (user.getProfileData()["Role"].ToString() == "Doctor")
               {
                    return View("~/Views/Admin/UpdateDoctorAccount.cshtml");
               }
               else
               {
                    // return View("~/Views/Admin/UpdatePatientCaregiverAccount.cshtml");
                    TempData["ErrorMessage"] = "User ID not found.";
                    return RedirectToAction("Dashboard");
               }
          }

          // POST: /Admin/UserProfile
          // POST: /Admin/updateAccount
          [HttpPost]
          public async Task<IActionResult> updateAccount(string uid, string email, string name, string role, string address, long? mobileNumber, string? department, string? specialization)
          {
               if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || mobileNumber == null || (string.IsNullOrEmpty(department) && string.IsNullOrEmpty(specialization)))
               {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
                    return RedirectToAction("LoadRoleForm", new { role = role });
               }

               // Create dictionary for updated data
               Dictionary<string, object> updatedUserData = new Dictionary<string, object>
               {
                    { "Email", email },
                    { "Name", name },
                    { "MobileNumber", mobileNumber },
                    { "Address", address },
                    { "Role", role }  // Always save the role
                    { "Role", role }
               };

               // Add role-specific fields
               if (role == "Nurse" && !string.IsNullOrEmpty(department))
               {
                    updatedUserData.Add("Department", department);
               }
               else if (role == "Doctor" && !string.IsNullOrEmpty(specialization))
               {
                    updatedUserData.Add("Specialization", specialization);
               }

               string result = await _adminManagement.updateStaffAccount(uid, updatedUserData);

               if (result == "Account updated successfully.")
               {
                    TempData["SuccessMessage"] = $"User with ID {uid} successfully updated!";
               }
               else
               {
                    TempData["ErrorMessage"] = $"User with ID {uid} failed to update.";
               }
               return RedirectToAction("Dashboard");
          }

          // POST: /Admin/ResetPassword
          [HttpPost]
          public async Task<IActionResult> resetPassword(string uid)
          {
               if (string.IsNullOrEmpty(uid))
               {
                    TempData["ErrorMessage"] = "User ID is required.";
                    return RedirectToAction("Dashboard");
               }

               string result = await _adminManagement.resetStaffPassword(uid);

               if (result == "Failed to reset password.")
               {
                    TempData["ErrorMessage"] = result;
               }
               else
               {
                    TempData["SuccessMessage"] = result;
               }

               return RedirectToAction("Dashboard");
          }

          // POST: /Admin/ResetPassword
          // POST: /Admin/DeleteAccount
          [HttpPost]
          public async Task<IActionResult> deleteAccount(string uid)
          {
               if (string.IsNullOrEmpty(uid))
               {
                    TempData["ErrorMessage"] = "User ID is required.";
                    return RedirectToAction("Dashboard");
               }

               string result = await _adminManagement.deleteAccount(uid);

               if (result == "Account deleted successfully.")
               {
                    TempData["SuccessMessage"] = $"Successfully deleted user with ID: {uid}";
               }
               else
               {
                    TempData["ErrorMessage"] = $"Failed to delete user with ID: {uid}";
               }

               return RedirectToAction("Dashboard");
          }

     }
}}
