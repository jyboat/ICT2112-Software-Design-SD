using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class LoginManagement
    {
        private readonly UserGateway UserGateway;
        private readonly IPassword passwordService;
        private readonly IEmail emailService;
        
        public LoginManagement(IPassword passwordService, IEmail emailService)
        {
            UserGateway = new UserGateway();
            this.passwordService = passwordService;
            this.emailService = emailService;
        }

        public async Task<User> authenticateUser(string userEmail, string userPassword)
        {
            // Find user account from firestore
            var user = await UserGateway.findUserByEmail(userEmail);

            // Checks if user exist after attempting to retrieve from FireBase
            if (user != null)
            {
                var storedPassword = user.getHashedPassword();
                // Compare the password
                if (passwordService.verifyPassword(userPassword, storedPassword))
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<User> getUserByID(string userID)
        {
            return await UserGateway.findUserByID(userID);
        }

        public async Task<bool> sendOTP(string email, string otpCode)
        {
            return await emailService.sendOTPEmail(email, otpCode);
        }
    }
}