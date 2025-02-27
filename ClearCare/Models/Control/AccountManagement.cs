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

            string newUserId = await _userGateway.GetNextUserId(); // Get the next available user ID

            // Create a new User object
            var newUser = new User(
                userID: newUserId,
                email: email,
                password: password,
                name: name,
                mobileNumber: mobileNumber,
                address: address,
                role: role
            );

            // Insert the new user into the database
            await _userGateway.InsertUser(newUser);

            return "Account created successfully.";
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