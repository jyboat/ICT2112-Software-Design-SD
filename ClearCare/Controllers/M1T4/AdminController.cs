using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Google.Cloud.Firestore;

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
               // Check for userRole
               var userRole = HttpContext.Session.GetString("Role");

               // Restrict access to only Admin
               if (userRole != "Admin")
               {
                    Console.WriteLine("You do not have permission to access this page.");
                    return RedirectToAction("displayLogin", "Login");
               }

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
               // Check for userRole
               var userRole = HttpContext.Session.GetString("Role");

               // Restrict access to only Admin
               if (userRole != "Admin")
               {
                    Console.WriteLine("You do not have permission to access this page.");
                    return RedirectToAction("displayLogin", "Login");
               }

               List<AuditLog> auditLogs = await _auditManagement.GetAllAuditLogsAsync();

               // Ensure ViewData is not null
               ViewData["AuditLogs"] = auditLogs ?? new List<AuditLog>();

               return View("~/Views/Admin/AuditView.cshtml");
          }

          // GET: /Admin/LoadCreateNurseAccount
          [HttpGet]
          public IActionResult LoadCreateNurseAccount()
          {
               // Check for userRole
               var userRole = HttpContext.Session.GetString("Role");

               // Restrict access to only Admin
               if (userRole != "Admin")
               {
                    Console.WriteLine("You do not have permission to access this page.");
                    return RedirectToAction("displayLogin", "Login");
               }

               return View("~/Views/Admin/CreateNurseAccount.cshtml");
          }

          // GET: /Admin/LoadCreateDoctorAccount
          [HttpGet]
          public IActionResult LoadCreateDoctorAccount()
          {
               // Check for userRole
               var userRole = HttpContext.Session.GetString("Role");

               // Restrict access to only Admin
               if (userRole != "Admin")
               {
                    Console.WriteLine("You do not have permission to access this page.");
                    return RedirectToAction("displayLogin", "Login");
               }

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
               // Check for userRole
               var userRole = HttpContext.Session.GetString("Role");
               var adminID = HttpContext.Session.GetString("UserID");

               // Restrict access to only Admin
               if (userRole != "Admin")
               {
                    Console.WriteLine("You do not have permission to access this page.");
                    return RedirectToAction("displayLogin", "Login");
               }

               if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || mobileNumber == null || (string.IsNullOrEmpty(department) && string.IsNullOrEmpty(specialization)))
               {
                    TempData["ErrorMessage"] = "Please fill in all required fields.";
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

               string result = await _adminManagement.createAccount(newUser!, password, adminID, _auditManagement);

               if (result == "Account created successfully.")
               {
                    TempData["SuccessMessage"] = result;
                    return RedirectToAction("Dashboard");
               }
               else
               {
                    TempData["ErrorMessage"] = result;
                    return RedirectToAction("LoadRoleForm", new { role });
               }
          }

          // GET: /Admin/UpdateProfilePage/{uid}
          [HttpGet("/UpdateProfilePage/{uid}")]
          public async Task<IActionResult> manageUsers(string uid)
          {
               // Check for userRole
               var userRole = HttpContext.Session.GetString("Role");

               // Restrict access to only Admin
               if (userRole != "Admin")
               {
                    Console.WriteLine("You do not have permission to access this page.");
                    return RedirectToAction("displayLogin", "Login");
               }

               if (string.IsNullOrEmpty(uid))
               {
                    TempData["ErrorMessage"] = "User ID is required.";
                    return RedirectToAction("Dashboard");
               }

               var user = await _adminManagement.retrieveUserByID(uid);
               ViewData["User"] = user;

               if (user.getProfileData()["Role"].ToString() == "Nurse")
               {
                    return View("~/Views/Admin/UpdateNurseAccount.cshtml");
               }
               else if (user.getProfileData()["Role"].ToString() == "Doctor")
               {
                    return View("~/Views/Admin/UpdateDoctorAccount.cshtml");
               }
               else if (user.getProfileData()["Role"].ToString() == "Patient")
               {
                    return View("~/Views/Admin/UpdatePatientAccount.cshtml");
               }
               else if (user.getProfileData()["Role"].ToString() == "Caregiver")
               {
                    return View("~/Views/Admin/UpdateCaregiverAccount.cshtml");
               }
               else
               {
                    TempData["ErrorMessage"] = "User ID not found.";
                    return RedirectToAction("Dashboard");
               }
          }

          // POST: /Admin/updateAccount
          [HttpPost]
          public async Task<IActionResult> updateAccount(string uid, string email, string name, string role, string address, long? mobileNumber, string? department, string? specialization, string? dateOfBirth)
          {
               var adminID = HttpContext.Session.GetString("UserID");

               if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || mobileNumber == null)
               {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
               }

               Dictionary<string, object> updatedUserData = new Dictionary<string, object>
               {
                    { "Email", email },
                    { "Name", name },
                    { "MobileNumber", mobileNumber ?? 0 },
                    { "Address", address },
                    { "Role", role },
                    { "DateOfBirth", dateOfBirth}
               };

               
               if (!string.IsNullOrEmpty(dateOfBirth))
               {
               if (DateTime.TryParseExact(dateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDob))
               {
                    updatedUserData["DateOfBirth"] = Timestamp.FromDateTime(DateTime.SpecifyKind(parsedDob, DateTimeKind.Utc));
               }
               else
               {
                    TempData["ErrorMessage"] = "Invalid Date of Birth format.";
                    return RedirectToAction("Dashboard");
               }
               }

               if (role == "Nurse" && !string.IsNullOrEmpty(department))
               {
                    updatedUserData.Add("Department", department);
               }
               else if (role == "Doctor" && !string.IsNullOrEmpty(specialization))
               {
                    updatedUserData.Add("Specialization", specialization);
               }

               string result = await _adminManagement.updateUserAccount(uid, updatedUserData, adminID, _auditManagement);

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
               var adminID = HttpContext.Session.GetString("UserID");

               if (string.IsNullOrEmpty(uid))
               {
                    TempData["ErrorMessage"] = "User ID is required.";
                    return RedirectToAction("Dashboard");
               }

               string result = await _adminManagement.resetStaffPassword(uid, adminID, _auditManagement);

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
}
