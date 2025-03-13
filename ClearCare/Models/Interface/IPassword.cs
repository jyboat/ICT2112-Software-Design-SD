using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IPassword
    {
        string hashPassword(string password);
        bool verifyPassword(string enteredPassword, string storedHashedPassword);
    }
}

