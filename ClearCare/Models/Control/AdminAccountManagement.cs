using System;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Newtonsoft.Json;

namespace ClearCare.Models.Control
{
     public class AdminAccountManagement
     {
          private readonly UserGateway _userGateway;

          // Constructor with dependency injection
          public AdminAccountManagement(UserGateway userGateway)
          {
               _userGateway = userGateway ?? throw new ArgumentNullException(nameof(userGateway));
          }

          // Method to create a new account
          public async Task<string> CreateStaffAccount(User newUser, String password)
          {
               // Create a new User object with the necessary data
               string newUserId = await _userGateway.InsertStaffUser(newUser, password);

               return newUserId != null ? "Account created successfully." : "Failed to create account.";
          }
     }
}