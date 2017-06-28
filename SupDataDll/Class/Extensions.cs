using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CloudManagerGeneralLib.Class
{
    public static class Extensions
    {
        public static string ToBase64(this byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Convert.ToBase64String(data));
            sb.Replace('+', '-');
            sb.Replace('/', '_');
            sb.Replace("=", string.Empty);

            return sb.ToString();
        }

        public static byte[] FromBase64(this string data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(data);
            sb.Append(string.Empty.PadRight((4 - data.Length % 4) % 4, '='));
            sb.Replace('-', '+');
            sb.Replace('_', '/');
            sb.Replace(",", string.Empty);

            return Convert.FromBase64String(sb.ToString());
        }

        public static string ToUTF8String(this byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] ToBytes(this string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static T[] CopySubArray<T>(this T[] source, int length, int offset = 0)
        {
            T[] result = new T[length];
            while (--length >= 0)
            {
                if (source.Length > offset + length)
                {
                    result[length] = source[offset + length];
                }
            }

            return result;
        }

        public static BigInteger FromMPINumber(this byte[] data)
        {
            // First 2 bytes defines the size of the component
            int dataLength = (data[0] * 256 + data[1] + 7) / 8;

            byte[] result = new byte[dataLength];
            Array.Copy(data, 2, result, 0, result.Length);

            return new BigInteger(result);
        }

        public static void CopyTo(this Stream inputStream, Stream outputStream, int bufferSize)
        {
            // For .Net 3.5
            // From http://referencesource.microsoft.com/#mscorlib/system/io/stream.cs,98ac7cf3acb04bb1
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, read);
            }
        }

        public static string EncodeUnicode(this string input)
        {
            string str = "";
            foreach (char chr in input)
            {
                if (((ushort)chr) < 127) str += chr;
                else str += "\\u" + ((ushort)chr).ToString("X");
            }
            return str;
        }

        public static void CleanNotWorkingThread(this List<Thread> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsAlive)
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        public static string ToHexString(this byte[] Bytes)
        {
            return BitConverter.ToString(Bytes).Replace("-", string.Empty);
        }

        public static byte[] HexToByte(this string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }
        static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
