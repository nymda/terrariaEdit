using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaEditor
{
    class crypto
    {
        private const string EncryptionKey = "h3y_gUyZ";
        public string name = "";
        public List<Byte> getRawData(string playerPath)
        {
            byte[] encrypted = File.ReadAllBytes(playerPath);
            byte[] decrypted = Decrypt(encrypted);
            return decrypted.ToList();
        }

        public static byte[] Decrypt(byte[] cipherText)
        {
            using (Rijndael rijndaelHandle = Rijndael.Create())
            {
                rijndaelHandle.Padding = PaddingMode.None;
                rijndaelHandle.Key = new UnicodeEncoding().GetBytes(EncryptionKey);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, rijndaelHandle.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                        cs.Close();                   
                    }
                    cipherText = ms.ToArray();
                }
            }
            return cipherText;
        }

        public void encryptAndSave(byte[] decryptedData, string path)
        {
            byte[] encrypted;
            using (Rijndael rijndaelHandle = Rijndael.Create())
            {
                rijndaelHandle.Padding = PaddingMode.None;
                rijndaelHandle.Key = new UnicodeEncoding().GetBytes(EncryptionKey);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, rijndaelHandle.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(decryptedData, 0, decryptedData.Length);
                        cs.Close();
                    }
                    encrypted = ms.ToArray();
                }
            }
            File.WriteAllBytes(path, encrypted);
        }
    }
}
