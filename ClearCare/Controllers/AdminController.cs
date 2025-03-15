using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Data;
using System.Reflection;

namespace ClearCare.Controllers
{
     public class AdminController : Controller
     {
          private readonly AdminManagement _adminManagement;

          public AdminController()
          {
               var userGateway = new UserGateway();
               _adminManagement = new AdminManagement(userGateway);
          }

          // GET: /Admin/Dashboard
          [HttpGet]
          public async Task<IActionResult> Dashboard()
          {
               var users = await _adminManagement.retrieveAllUsers();

               if (users != null)
               {
                    var sortedUsers = users.OrderBy(u => u.getProfileData()["UserID"]).ToList();
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
                    return View("~/Views/Admin/CreatePatientCaregiverAccount.cshtml");
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
               // else if (role == "Patient")
               // {
               //      newUser = new Patient("", email, password, name, (long)mobileNumber, address, role);
               // }
               // else if (role == "Caregiver")
               // {
               //      newUser = new Caregiver("", email, password, name, (long)mobileNumber, address, role);
               // }

               string result = await _adminManagement.CreateStaffAccount(newUser!, password);

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

          // GET: /Admin/UpdateProfilePage/{uid}
          [HttpGet("/UpdateProfilePage/{uid}")]
          public async Task<IActionResult> manageUsers(string uid)
          {
               if (string.IsNullOrEmpty(uid))
               {
                    TempData["ErrorMessage"] = "User ID is required.";
                    return RedirectToAction("Dashboard"); // Redirect to Dashboard or another page
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
          [HttpPost]
          public async Task<IActionResult> updateAccount(string uid, string email, string name, string role, string address, long? mobileNumber, string? department, string? specialization)
          {
               if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || mobileNumber == null || (string.IsNullOrEmpty(department) && string.IsNullOrEmpty(specialization)))
               {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
                    return RedirectToAction("LoadRoleForm", new { role = role });
               }
               // Role-specific validation
               if (role == "Nurse" && string.IsNullOrEmpty(department))
               {
                    ViewBag.ErrorMessage = "Department is required for Nurse accounts.";
                    return RedirectToAction("LoadRoleForm", new { role = role });
               }
               else if (role == "Doctor" && string.IsNullOrEmpty(specialization))
               {
                    ViewBag.ErrorMessage = "Specialization is required for Doctor accounts.";
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

               if (result == "Password reset successful.")
               {
                    TempData["SuccessMessage"] = $"Password reset for user with ID: {uid}";
               }
               else
               {
                    TempData["ErrorMessage"] = $"Password reset failed for user with ID: {uid}";
               }

               return RedirectToAction("Dashboard");
          }

          // POST: /Admin/ResetPassword
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