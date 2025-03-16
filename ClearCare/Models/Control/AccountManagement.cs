using System;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class AccountManagement
    {
        private readonly UserGateway _userGateway;

        // Constructor with dependency injection
        public AccountManagement(UserGateway userGateway)
        {
            _userGateway = userGateway ?? throw new ArgumentNullException(nameof(userGateway));
        }

        // Method to create a new account
        public async Task<string> CreateAccount(User newUser, String password)
        {
            // First, check if an account with the same email already exists
            var existingUser = await _userGateway.findUserByEmail((string)newUser.getProfileData()["Email"]);
            if (existingUser != null)
            {
                return "Account already exists.";
            }

            // Create a new User object with the necessary data
            string newUserId = await _userGateway.InsertUser(newUser, password);

            return newUserId != null ? "Account created successfully." : "Failed to create account.";
        }

        // Method to check if an account already exists
        public async Task<string> CheckExistingAccount(string email)
        {
            // Check the database for an existing user with the same email
            var user = await _userGateway.findUserByEmail(email);
            return user != null ? "Account exists." : "Account does not exist.";
        }
    }
}