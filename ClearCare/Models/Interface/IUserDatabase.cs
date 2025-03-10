using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IAdminDatabase
    {
        Task<User> FindUserByID(string userID);
        Task<User> FindUserByEmail(string email);
        Task<string> FindUserNameByID(string userID);
        Task<List<User>> GetAllUsers();
    }
}

