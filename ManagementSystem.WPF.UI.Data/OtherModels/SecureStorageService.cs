using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ManagementSystem.WPF.UI.Data.OtherModels
{
    public class SecureStorageService
    {
        // Optional entropy acts as an additional password secret known only to your app code
        private static readonly byte[] OptionalEntropy = Encoding.UTF8.GetBytes("YourAppUniqueEntropyString");
        private readonly string _filePath;

        public SecureStorageService()
        {
            // Store in AppData/Local/YourAppName/token.dat
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ManagementSystem");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "token.dat");
        }

        public void SaveToken(string token)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(token);

            // Encrypt the token using current Windows user scope
            byte[] encryptedBytes = ProtectedData.Protect(plaintextBytes, OptionalEntropy, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(_filePath, encryptedBytes);
        }

        public string ReadToken()
        {
            if (!File.Exists(_filePath)) return null;

            try
            {
                byte[] encryptedBytes = File.ReadAllBytes(_filePath);

                // Decrypt using the exact same Windows user context
                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, OptionalEntropy, DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (CryptographicException)
            {
                // Decryption fails if accessed by a different Windows user or modified
                return null;
            }
        }

        public void ClearToken()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }

}
