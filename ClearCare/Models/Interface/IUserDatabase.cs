using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IAdminDatabase
    {
        Task<User> findUserByID(string userID);
        Task<User> findUserByEmail(string email);
        Task<string> findUserNameByID(string userID);
        Task<List<User>> getAllUsers();
    }
}

