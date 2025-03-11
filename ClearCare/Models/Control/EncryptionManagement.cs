using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using ClearCare.Models.Interface;

namespace ClearCare.Models.Control
{
    public class EncryptionManagement : IEncryption, IPassword
    {
        private readonly byte[] Key;
        private readonly byte[] IV;

        public EncryptionManagement()
        {
            // Hardcoded key and IV (Should be stored securely in a configuration file)
            Key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes (128-bit)
            IV = Encoding.UTF8.GetBytes("1234567890123456");  // 16 bytes (128-bit)
        }

        // ==========================
        // AES ENCRYPTION FOR MEDICAL DATA
        // ==========================

        public string EncryptMedicalData(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public string DecryptMedicalData(string encryptedText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        // ==========================
        // BCRYPT HASHING FOR PASSWORDS
        // ==========================

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
        }
    }
}
