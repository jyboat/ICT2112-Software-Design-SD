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
        public async Task<string> CreateAccount(string email, string password, string name, long mobileNumber, string address, string role)
        {
            // First, check if an account with the same email already exists
            var existingUser = await _userGateway.FindUserByEmail(email);
            if (existingUser != null)
            {
                return "Account already exists.";
            }

            // Create a new User object with the necessary data
            string newUserId = await _userGateway.InsertUser(email, password, name, mobileNumber, address, role);

            return newUserId != null ? "Account created successfully." : "Failed to create account.";
        }

        // Method to check if an account already exists
        public async Task<string> CheckExistingAccount(string email)
        {
            // Check the database for an existing user with the same email
            var user = await _userGateway.FindUserByEmail(email);
            return user != null ? "Account exists." : "Account does not exist.";
        }
    }
}