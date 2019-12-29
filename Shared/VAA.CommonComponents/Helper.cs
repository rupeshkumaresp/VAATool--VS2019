using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace VAA.CommonComponents
{
    /// <summary>
    /// General Helper class
    /// </summary>
    public class Helper
    {
        public static string Get8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }

        public static string Get3Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 1000;
            return String.Format("{0:D3}", random);
        }

        public static bool IsValidExcelFormat(Stream stream)
        {
            bool valid = true;
            try
            {
                var package = new ExcelPackage(stream);
            }
            catch (Exception ex)
            {
                valid = false;
            }

            return valid;
        }


        public static void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0) return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}
