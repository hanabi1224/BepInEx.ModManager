using System.Security.Cryptography;
using System.Text;

namespace BepInEx.ModManager.Server
{
    public static class HashUtils
    {
        public static string GetMD5String(string input)
        {
            using MD5 hasher = MD5.Create();
            byte[] hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            return ByteArrayToString(hashBytes);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}
