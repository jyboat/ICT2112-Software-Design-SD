using System;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using System.Security.Cryptography;
using System.Text;

namespace ClearCare.Models.Control
{
     public class AdminManagement
     {
          private readonly UserGateway _userGateway;

          public AdminManagement(UserGateway userGateway)
          {
               _userGateway = userGateway;
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
          public async Task<string> createAccount(User newUser, String password)
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
               var mobileNumber = Convert.ToInt64(newUser.getProfileData()["MobileNumber"]);
               var address = newUser.getProfileData()["Address"]?.ToString();
               var role = newUser.getProfileData()["Role"]?.ToString();

               if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(role))
               {
                    return "Password, Name, Address and Role is required.";
               }

               // Create a new User object with the necessary data
               string newUserId = await _userGateway.InsertUser(newUser, password);

               return newUserId != null ? "Account created successfully." : "Failed to create account.";
          }


          // Method to update user account
          public async Task<string> updateStaffAccount(string uid, Dictionary<string, object> profileData)
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

               return result ? "Account updated successfully." : "Failed to update account.";
          }

          // Method to reset password
          public async Task<string> resetStaffPassword(string uid)
          {
               var user = await _userGateway.findUserByID(uid);
               if (user == null)
               {
                    return "Account does not exist.";
               }

               var result = await _userGateway.resetPassword(uid, GeneratePassword());

               return result ? "Password reset successful." : "Failed to reset password.";
          }

          // Method to delete account
          public async Task<string> deleteAccount(string uid)
          {
               var user = await _userGateway.findUserByID(uid);
               if (user == null)
               {
                    return "Account does not exist.";
               }

               var result = await _userGateway.deleteUser(uid);

               return result ? "Account deleted successfully." : "Failed to delete account.";
          }
     }
}