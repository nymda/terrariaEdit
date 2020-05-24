using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaEditor
{
    public class crypto
    {
        private const string EncryptionKey = "h3y_gUyZ";
        public FileInfo fi;

        //fuck crypto lol

        public byte[] decryptNew(byte[] encrypted, int length)
        {
            using (var rijndaelManaged = new RijndaelManaged { Padding = PaddingMode.None })
            using (var memoryStream = new MemoryStream(encrypted))
            {
                var bytes = new UnicodeEncoding().GetBytes(EncryptionKey);
                using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read))
                using (var reader = new BinaryReader(cryptoStream))
                {
                    return reader.ReadBytes(length);
                }
            }
        }
        public void encryptAndSave(byte[] decryptedData, string path)
        {
            byte[] encrypted;
            using (var rijndaelManaged = new RijndaelManaged { Padding = PaddingMode.None })
            using (var memoryStream = new MemoryStream(decryptedData))
            {
                var bytes = new UnicodeEncoding().GetBytes(EncryptionKey);
                using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(bytes, bytes), CryptoStreamMode.Read))
                using (var reader = new BinaryReader(cryptoStream))
                {
                    encrypted = reader.ReadBytes(decryptedData.Length);
                }
            }
            File.WriteAllBytes(path, encrypted);
        }
    }
}
