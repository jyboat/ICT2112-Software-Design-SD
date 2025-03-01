using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IUserDetails
    {
        Task<User> getUserDetails(string userID);
    }
}

