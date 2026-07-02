using System.Security.Cryptography;
using System.Text;
using UnityEngine;


namespace Eloi.TBIO
{

    public class TBIO_SHA256
    {

        public static void TextToSHA256(string text, out string textHashed)
        {

            // Convert the password string to a byte array
            byte[] passwordBytes = Encoding.UTF8.GetBytes(text);

            // Compute the SHA-256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash byte array to a hexadecimal string
                StringBuilder hashStringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashStringBuilder.Append(b.ToString("x2")); // Convert each byte to hex format
                }
                textHashed = hashStringBuilder.ToString(); // Return the resulting hash
                return;
            }
        }
    }
}
