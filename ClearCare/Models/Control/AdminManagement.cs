using System;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Newtonsoft.Json;

namespace ClearCare.Models.Control
{
     public class AdminManagement
     {
          private readonly UserGateway _userGateway;

          // Constructor with dependency injection
          public AdminManagement(UserGateway userGateway)
          {
               _userGateway = userGateway ?? throw new ArgumentNullException(nameof(userGateway));
          }

          public async Task<List<User>> RetrieveAllUsers() => await _userGateway.getAllUsers();

          // Method to create a new account
          public async Task<string> CreateStaffAccount(User newUser, String password)
          {
               // Create a new User object with the necessary data
               string newUserId = await _userGateway.InsertStaffUser(newUser, password);

               return newUserId != null ? "Account created successfully." : "Failed to create account.";
          }
     }
}