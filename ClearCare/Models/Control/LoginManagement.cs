using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;

namespace ClearCare.Models.Control
{
    public class LoginManagement
    {
        private readonly UserGateway UserGateway;
        
        public LoginManagement()
        {
            UserGateway = new UserGateway();
        }

        public async Task<User> AuthenticateUser(string email, string password)
        {
            // Find user account from firestore
            var user = await UserGateway.FindUserByEmail(email);

            // If user exists and password is correct, return user
            if (user != null && user.ValidatePassword(password))
            {
                return user;
            }

            return null;
        }
    }
}