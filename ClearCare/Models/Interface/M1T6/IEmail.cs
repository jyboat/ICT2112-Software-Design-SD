using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IEmail
    {
        Task<bool> sendOTPEmail(string toEmail, string otpCode);
    }
}

