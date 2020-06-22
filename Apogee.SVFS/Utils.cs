using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Apogee.SVFS
{
    public static class Utils
    {
        public static void BlockCopyTo(this Stream a, Stream to, ulong size)
        {
            var bytesPerCycle = (ulong) 8192;
            var numCycles = size / bytesPerCycle;

            void ReadChunk(ulong size)
            {
                var buf = new byte[size];
                a.Read(buf);
                to.Write(buf);
            }

            for (ulong i = 0; i < numCycles; i++)
            {
                ReadChunk(bytesPerCycle);
            }

            var remainder = size - (numCycles * bytesPerCycle);
            ReadChunk(remainder);
        }

        public static string Md5Hash(this Stream a)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(a);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string Md5Hash(this string s)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(s));

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
        
        public static string Md5Hash(this byte[] s)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(s);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}