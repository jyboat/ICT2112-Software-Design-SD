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
        
        public LoginManagement(IPassword passwordService)
        {
            UserGateway = new UserGateway();
            this.passwordService = passwordService;
        }

        public async Task<User> AuthenticateUser(string userEmail, string userPassword)
        {
            // Find user account from firestore
            var user = await UserGateway.FindUserByEmail(userEmail);

            // Checks if user exist after attempting to retrieve from FireBase
            if (user != null)
            {
                var storedPassword = user.GetHashedPassword();
                // Compare the password
                if (passwordService.VerifyPassword(userPassword, storedPassword))
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<User> GetUserByID(string userID)
        {
            // Find user account from firestore
            var user = await UserGateway.FindUserByID(userID);

            return user;
        }
    }
}