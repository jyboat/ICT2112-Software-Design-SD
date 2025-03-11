using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IPassword
    {
        string HashPassword(string password);
        bool VerifyPassword(string enteredPassword, string storedHashedPassword);
    }
}

