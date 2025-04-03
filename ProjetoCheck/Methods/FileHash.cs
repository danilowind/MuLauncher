using System.IO;
using System.Security.Cryptography;

namespace ProjetoCheck.Methods
{
    class FileHash
    {
        public static string HashFile(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hashBytes = sha256.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
