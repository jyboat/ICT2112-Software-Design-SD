using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Control
{
    public class ProfileManagement
    {
        private readonly UserGateway _userGateway;

        public ProfileManagement()
        {
            _userGateway = new UserGateway();
        }

        // Fetch user details based on UserID, called by ProfileController
        public async Task<User> getUserDetails(string userID)
        {
            return await _userGateway.FindUserByID(userID);
        }

    }
}
