using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Interface
{
    public interface IDelegateDatabase
    {
        Task<bool> updateUser(string userID, Dictionary<string, object> updatedFields);
    }
}