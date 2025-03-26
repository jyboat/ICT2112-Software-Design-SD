using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IAccountDatabase
    {
        Task<string> InsertUser(string email, string password, string name, long mobileNumber, string address, string role);
        Task<string> UpdateUser(string email, string password, string name, long mobileNumber, string address, string role);
        Task<string> DeleteUser(string email, string password, string name, long mobileNumber, string address, string role);
    }
}

