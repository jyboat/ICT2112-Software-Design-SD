using System;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class AccountManagement
    {
        private readonly UserGateway _userGateway;
        private readonly IPassword _passwordService;
        // Constructor with dependency injection
        public AccountManagement(UserGateway userGateway, IPassword passwordService)
        {
            _userGateway = userGateway ?? throw new ArgumentNullException(nameof(userGateway));
            _passwordService = passwordService;
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

        public async Task<bool> ResetPassword(string email, string newPassword, HttpContext httpContext)
        {
            var resetConfirmed = httpContext.Session.GetString("ResetConfirmed");
            if (resetConfirmed != "true")
            {
                return false;
            }

            var user = await _userGateway.findUserByEmail(email);
            if (user == null)
            {
                return false;
            }

            // Use getProfileData() to get the UserID without making getUserID() public
            var profileData = user.getProfileData();
            if (!profileData.ContainsKey("UserID"))
            {
                return false; // UserID not found
            }

            string userId = profileData["UserID"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return false; // Ensure it's not null or empty
            }

            // Hash the password
            string hashedPassword = _passwordService.hashPassword(newPassword);
            bool isUpdated = await _userGateway.resetPassword(userId, hashedPassword);

            if (isUpdated)
            {
                httpContext.Session.Remove("ResetConfirmed");
                return true;
            }

            return false;
        }
    }
}