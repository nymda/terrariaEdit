using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaEditor
{
    class lib
    {
        private const string EncryptionKey = "h3y_gUyZ";
        private const ulong MagicNumber = 27981915666277746;
        public void getRawData(string playerPath)
        {
            byte[] encrypted = File.ReadAllBytes(playerPath);
            byte[] decrypted = Decrypt(encrypted);
            List<int> intlist = new List<int> { };
            foreach(byte b in decrypted)
            {
                intlist.Add(Convert.ToInt32(b));
            }
            string done = string.Join(",", intlist);
            Console.WriteLine(done);
            Console.WriteLine(intlist.Count);
        }

        public static byte[] Decrypt(byte[] cipherText)
        {
            byte[] cipherBytes = cipherText;
            using (Rijndael rijndaelHandle = Rijndael.Create())
            {
                rijndaelHandle.Padding = PaddingMode.None;
                rijndaelHandle.Key = new UnicodeEncoding().GetBytes(EncryptionKey);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, rijndaelHandle.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = ms.ToArray();
                }
            }
            return cipherText;
        }
    }
}
