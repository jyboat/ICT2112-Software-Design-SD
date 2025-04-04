using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IAccountDatabase
    {
        Task<string> insertUser(string email, string password, string name, long mobileNumber, string address, string role);
        Task<string> updateUser(string email, string password, string name, long mobileNumber, string address, string role);
        Task<string> deleteUser(string email, string password, string name, long mobileNumber, string address, string role);
    }
}

