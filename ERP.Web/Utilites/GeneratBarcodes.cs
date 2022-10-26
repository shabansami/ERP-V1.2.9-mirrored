using BarcodeLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ERP.Web.Utilites
{
    public static class GeneratBarcodes
    {
        public static string GenerateRandomBarcode()
        {
            string randomNumber = GenerateNumber();

            //if (StudentBookingsService.NumberExists(randomNumber))
            //    GenerateRandomBarcode();

            return randomNumber;
        }
        /// <summary>
        /// Generate a random 10 digit number
        /// </summary>
        /// <returns></returns>
        public static string GenerateNumber()
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 7; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }
        public static string GenerateBarcode(string str)
        {
            Barcode barcode = new Barcode();

            //Show the string
            barcode.IncludeLabel = true;
            barcode.Alignment = AlignmentPositions.CENTER;

            Image img = barcode.Encode(TYPE.CODE128, str, Color.Black, Color.White, 150, 40);

            var imageBas64= ImageToByteArray(img);
            return Convert.ToBase64String(imageBas64);
        }
        public static byte[] ImageToByteArray(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}

