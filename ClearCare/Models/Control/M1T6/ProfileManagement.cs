using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models.Entities;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class ProfileManagement : IUserDetails
    {
        private readonly UserGateway _userGateway;

        public ProfileManagement()
        {
            _userGateway = new UserGateway();
        }

        // Fetch user details based on UserID, called by ProfileController
        public async Task<User> getUserDetails(string userID)
        {
            return await _userGateway.findUserByID(userID);
        }

        // Update user profile with selected fields, called by ProfileController
        public async Task<bool> editUserDetails(string userID, Dictionary<string, object> updatedFields)
        {
            return await _userGateway.updateUser(userID, updatedFields);
        }

        public async Task<bool> emailExists(string email, string userID) 
        {
            return await _userGateway.checkDuplicateEmail(email, userID);
        }
        public async Task<bool> mobileExists(long mobileNumber, string userID)
        {
            return await _userGateway.checkDuplicateMobile(mobileNumber, userID);
        }

    }
}
