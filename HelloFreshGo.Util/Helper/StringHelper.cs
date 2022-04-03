using System;
using System.Security.Cryptography;
using System.Text;

namespace HelloFreshGo.Util.Helper
{
    public interface IStringHelper
    {
        string DecryptCredentials(string credentials);
    }

    public class StringHelper : IStringHelper
    {
        byte[] key = { 7, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8,
                           1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };


        private static string GetString(byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }

        private byte[] Decrypt(byte[] data, byte[] key)
        {
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                csp.KeySize = 256;
                csp.BlockSize = 128;
                csp.Key = key;
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform decrypter = csp.CreateDecryptor();
                return decrypter.TransformFinalBlock(data, 0, data.Length);
            }
        }

        public string DecryptCredentials(string credentials)
        {
            var getEncryptedByte = Convert.FromBase64String(credentials);
            byte[] dec = Decrypt(getEncryptedByte, key);

            return GetString(dec);
        }
    }
}
