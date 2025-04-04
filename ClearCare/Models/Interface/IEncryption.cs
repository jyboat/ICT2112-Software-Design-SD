using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Control;

namespace ClearCare.Models.Interface
{
    public interface IEncryption
    {
        string encryptMedicalData(string plainText);
        string decryptMedicalData(string encryptedText);
    }
}

