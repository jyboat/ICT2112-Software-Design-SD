using System;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using ClearCare.Models.Interface;
using System.Security.Cryptography;
using System.Text;

namespace ClearCare.Models.Control
{
     public class AdminManagement : IUserList
     {
          private readonly UserGateway _userGateway;
          private readonly EmailService _emailService;

          public AdminManagement(UserGateway userGateway)
          {
               _userGateway = userGateway;
               _emailService = new EmailService();
          }

          public static string GeneratePassword(int length = 12)
          {
               string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
               string LowerCase = "abcdefghijklmnopqrstuvwxyz";
               string Digits = "0123456789";
               string SpecialChars = "!@#$%^&*()_-+=<>?";

               if (length < 8) throw new ArgumentException("Password length must be at least 8 characters.");

               string allChars = UpperCase + LowerCase + Digits + SpecialChars;
               StringBuilder password = new StringBuilder();
               byte[] randomBytes = new byte[length];

               using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
               {
                    rng.GetBytes(randomBytes);
               }

               for (int i = 0; i < length; i++)
               {
                    password.Append(allChars[randomBytes[i] % allChars.Length]);
               }

               return password.ToString();
          }

          public async Task<List<User>> retrieveAllUsers() => await _userGateway.getAllUsers();
          public async Task<User> retrieveUserByID(string uid) => await _userGateway.findUserByID(uid);

          // Method to create a new account
          public async Task<string> createAccount(User newUser, IAuditSubject auditLog)
          {
               var email = newUser.getProfileData()["Email"]?.ToString();
               if (string.IsNullOrEmpty(email))
               {
                    return "Email is required.";
               }

               // check if an account with the same email already exists
               var existingUser = await _userGateway.findUserByEmail(email);
               if (existingUser != null)
               {
                    return "Account already exists.";
               }

               // validate that the fields are not empty, then create user
               var name = newUser.getProfileData()["Name"]?.ToString();
               var mobileNumber = Convert.ToInt64(newUser.getProfileData()["MobileNumber"]).ToString();
               var address = newUser.getProfileData()["Address"]?.ToString();
               var role = newUser.getProfileData()["Role"]?.ToString();

               if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(mobileNumber))
               {
                    return "Name, Phone Number, Address and Role is required.";
               }

               var newPassword = GeneratePassword();

               // Create a new User object with the necessary data
               string newUserId = await _userGateway.InsertUser(newUser, newPassword);

               // Insert audit log after account creation
               string auditResult = await auditLog.InsertAuditLog("Created new account", newUserId);

               var emailBody = $"""
                         Hello {newUser.getProfileData()["Name"]},

                         Your {newUser.getProfileData()["Role"]} account with {newUser.getProfileData()["Email"]} has been created.

                         Your temporary password is: {newPassword}

                         For security reasons, you will be required to change this password when you next log in.

                         If you did not request this change, please contact Clear Care Customer Service immediately.

                         Best regards,
                         ClearCare Support Team
                    """;

               bool sendStatus = await _emailService.sendEmail(newUser.getProfileData()["Email"].ToString()!, "New Account Created - Action Required", emailBody);

               if (!sendStatus && newUserId != null)
               {
                    return $"Failed to send email notification for ${newUserId}.";
               }

               return newUserId != null ? "Account created successfully." : "Failed to create account.";
          }


          // Method to update user account
          public async Task<string> updateStaffAccount(string uid, Dictionary<string, object> profileData, IAuditSubject auditLog)
          {
               if (string.IsNullOrEmpty(uid))
               {
                    return "User ID is required.";
               }

               var user = await _userGateway.findUserByID(uid);
               if (user == null)
               {
                    return "Account does not exist.";
               }

               var result = await _userGateway.updateUser(uid, profileData);

               // Insert audit log after successful account update
               string auditResult = await auditLog.InsertAuditLog("Updated staff account", uid);

               return result ? "Account updated successfully." : "Failed to update account.";
          }

          // Method to reset password
          public async Task<string> resetStaffPassword(string uid, IAuditSubject auditLog)
          {
               var user = await _userGateway.findUserByID(uid);
               if (user == null)
               {
                    return "Account does not exist.";
               }

               var newPassword = GeneratePassword();
               // Set the temporary password and mark account for required password change
               var result = await _userGateway.resetPassword(uid, newPassword);

               if (result)
               {
                    // Insert audit log after successful password reset
                    string auditResult = await auditLog.InsertAuditLog("Reset staff password", uid);

                    // Update user profile to require password change
                    var profileData = user.getProfileData();

                    // Send email with temporary password
                    var emailBody = $"""
                         Hello {profileData["Name"]},

                         Your password for {profileData["Email"]} has been reset.

                         Your temporary password is: {newPassword}

                         For security reasons, you will be required to change this password when you next log in.

                         If you did not request this change, please contact Clear Care Customer Service immediately.

                         Best regards,
                         ClearCare Support Team
                    """;

                    bool sendStatus = await _emailService.sendEmail(profileData["Email"].ToString()!, "Password Reset - Action Required", emailBody);

                    if (!sendStatus)
                    {
                         Console.WriteLine("Password reset successful but failed to send email notification.");
                         return "Password reset successful but failed to send email notification.";
                    }

                    Console.WriteLine("Password reset successful. User will be required to change password on next login.");
                    return "Password reset successful. User will be required to change password on next login.";
               }

               return "Failed to reset password.";
          }

          // Method to delete account
          public async Task<string> deleteAccount(string uid, IAuditSubject auditLog)
          {
               var user = await _userGateway.findUserByID(uid);
               if (user == null)
               {
                    return "Account does not exist.";
               }

               var result = await _userGateway.deleteUser(uid);

               // Insert audit log after successful account deletion
               string auditResult = await auditLog.InsertAuditLog("Deleted account", uid);

               return result ? "Account deleted successfully." : "Failed to delete account.";
          }
     }
}