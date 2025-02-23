using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using ClearCare.Models.Control;

namespace ClearCare.Models.Control
{
    public class LoginManagement
    {
        private readonly UserGateway UserGateway;
        private readonly EncryptionManagement encryptionManagement;
        
        public LoginManagement()
        {
            UserGateway = new UserGateway();
            encryptionManagement = new EncryptionManagement();
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
                if (encryptionManagement.VerifyPassword(userPassword, storedPassword))
                {
                    return user;
                }
            }
            return null;
        }
    }
}