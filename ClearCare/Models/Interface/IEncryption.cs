using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IEncryption
    {
        string EncryptMedicalData(string plainText);
        string DecryptMedicalData(string encryptedText);
    }
}

