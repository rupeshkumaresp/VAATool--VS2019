using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VAA.CommonComponents
{
    /// <summary>
    /// Encryption Helper - MD5 encryption, description
    /// </summary>
    public class EncryptionHelper
    {
        public string EncryptPassword(string password)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sBuilder = new StringBuilder();
                foreach (byte bytedata in data)
                {
                    sBuilder.Append(bytedata.ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string Encrypt(string plainText)
        {
            string passPhrase = "Espvaa@pr";// can be any string
            string saltValue = "s@1tValue";// can be any string
            string hashAlgorithm = "SHA-256";// can be "MD5"
            int passwordIterations = 2;// can be any number
            string initVector = "@1B2c3D4e5F6g7H8";// must be 16 bytes
            int keySize = 256;// can be 192 or 128 or 256
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            // Finish encrypting.
            cryptoStream.FlushFinalBlock();
            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();
            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();
            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);
            // Return encrypted string.
            return cipherText;
        }

        public static string Decrypt(string cipherText)
        {
            string passPhrase = "Espvaa@pr";// can be any string
            string saltValue = "s@1tValue";// can be any string
            string hashAlgorithm = "SHA-256";// can be "MD5"
            int passwordIterations = 2;// can be any number
            string initVector = "@1B2c3D4e5F6g7H8";// must be 16 bytes
            int keySize = 256;// can be 192 or 128 or 256
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            // Return decrypted string.   
            return plainText;
        }
    }
}
