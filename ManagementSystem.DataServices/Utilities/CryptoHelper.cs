
namespace ManagementSystem.DataServices.Utilities
{
    /// <summary>
    /// Simple, modern AES-GCM string encryption helpers using PBKDF2 for key derivation.
    /// Produces a Base64 encoded blob: [salt(16)] [nonce(12)] [ciphertext] [tag(16)]
    /// </summary>
    public static class CryptoHelper
    {
        private const int SaltSize = 32;      // bytes
        private const int NonceSize = 12;     // bytes for AES-GCM
        private const int TagSize = 16;       // bytes for AES-GCM tag
        private const int KeySize = 32;       // 256-bit key
        private const int Iterations = 100_000;

        public static string[] EncryptString(string plaintext)
        {
            if (plaintext is null) throw new ArgumentNullException(nameof(plaintext));

            string[] result = new string[2];

            // Menghasilkan salt 32-byte (256-bit)
            byte[] salt = RandomNumberGenerator.GetBytes(32);
            result[0] = Convert.ToBase64String(salt);
            result[1] = EncryptString(plaintext, salt);

            return result;
        }

        public static string EncryptString(string plaintext, string saltText)
        {
            if (plaintext is null) throw new ArgumentNullException(nameof(plaintext));
            if (saltText is null) throw new ArgumentNullException(nameof(saltText));

            return EncryptString(plaintext, Convert.FromBase64String(saltText));
        }

        public static string EncryptString(string plaintext, byte[] salt)
        {
            if (plaintext is null) throw new ArgumentNullException(nameof(plaintext));
            if (salt.Length == 0) throw new ArgumentNullException(nameof(salt));

            byte[] nonce = new byte[NonceSize];

            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(nonce);

            byte[] key = DeriveKey(plaintext, salt);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[TagSize];

            using (var aes = new AesGcm(key, TagSize))
            {
                aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            }

            var byteResult = new byte[SaltSize + NonceSize + ciphertext.Length + TagSize];
            Buffer.BlockCopy(salt, 0, byteResult, 0, SaltSize);
            Buffer.BlockCopy(nonce, 0, byteResult, SaltSize, NonceSize);
            Buffer.BlockCopy(ciphertext, 0, byteResult, SaltSize + NonceSize, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, byteResult, SaltSize + NonceSize + ciphertext.Length, TagSize);

            return Convert.ToBase64String(byteResult);
        }

        /// <summary>
        /// Decrypts the Base64 blob produced by EncryptString using the same password.
        /// </summary>
        public static string DecryptString(string encryptedBase64, string password)
        {
            if (encryptedBase64 is null) throw new ArgumentNullException(nameof(encryptedBase64));
            if (password is null) throw new ArgumentNullException(nameof(password));

            byte[] blob = Convert.FromBase64String(encryptedBase64);

            if (blob.Length < SaltSize + NonceSize + TagSize)
                throw new ArgumentException("Invalid encrypted data.", nameof(encryptedBase64));

            var salt = new byte[SaltSize];
            var nonce = new byte[NonceSize];

            Buffer.BlockCopy(blob, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(blob, SaltSize, nonce, 0, NonceSize);

            int ciphertextLength = blob.Length - SaltSize - NonceSize - TagSize;
            var ciphertext = new byte[ciphertextLength];
            var tag = new byte[TagSize];

            Buffer.BlockCopy(blob, SaltSize + NonceSize, ciphertext, 0, ciphertextLength);
            Buffer.BlockCopy(blob, SaltSize + NonceSize + ciphertextLength, tag, 0, TagSize);

            byte[] key = DeriveKey(password, salt);

            var plaintextBytes = new byte[ciphertextLength];
            try
            {
                using (AesGcm aes = new AesGcm(key, TagSize))
                {
                    aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);
                }
            }
            catch (CryptographicException ex)
            {
                // authentication failed (bad password or corrupted data)
                throw new CryptographicException("Decryption failed. Data may be corrupted or password is incorrect.", ex);
            }

            return Encoding.UTF8.GetString(plaintextBytes);
        }

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return kdf.GetBytes(KeySize);
            }
        }
    }
}