using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ERP.Web.Utilites
{
    public class Utility
    {
        public static DateTime GetDateTime() => DateTime.UtcNow.AddHours(2);

        public static string GenerateRandomBarcode(string barCode)
        {
            string randomNumber = RandomGenerator.GenerateNumber();

            //if (barCode.NumberExists(randomNumber))
            //    GenerateRandomBarcode();

            return randomNumber;
        }
        //تحويل بيانات الفاتورة الى كيو ار كود QRCode
        public static string ConvertToQRCode(string txt)
        {
            //QRCoder.QRCodeGenerator qrGenerator =
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCode qrCode = new QRCode(qrGenerator.CreateQrCode(txt, QRCodeGenerator.ECCLevel.Q));
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(qrCode.GetGraphic(6));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            var base64String = Convert.ToBase64String(ms.ToArray());
            return Convert.ToString(base64String);
            //}
            //    Dim qrGenerator As New QRCoder.QRCodeGenerator
            //    Dim qrCode As New QRCoder.QRCode(qrGenerator.CreateQrCode(txt, QRCoder.QRCodeGenerator.ECCLevel.Q))
            //    Dim bm As New Drawing.Bitmap(qrCode.GetGraphic(6))
            //    Dim ms As New IO.MemoryStream
            //    bm.Save(ms, Drawing.Imaging.ImageFormat.Jpeg)
            //    Dim base64String = Convert.ToBase64String(ms.ToArray)
            //    Return Convert.ToString("data:image/png;base64,") & base64String
            //End Function

        }

        #region Send mail
        public static bool SendMail(string to, string subj, string body, bool isHtml = false, List<Attachment> attachments = null)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("testmail20172018@gmail.com", "135791113"),
                    //DeliveryMethod=SmtpDeliveryMethod.Network
                };
                MailMessage Msg = new MailMessage("testmail20172018@gmail.com", to, subj, body);

                if (attachments != null)
                {
                    foreach (var item in attachments)
                    {
                        Msg.Attachments.Add(item);
                    }
                }
                Msg.IsBodyHtml = isHtml;
                client.Send(Msg);
                return true;
            }
            catch (Exception ex)
            {
                var e = ex.Message;
                return false;
            }


        }

        #endregion

        public static int GetRandom(List<int> records, int max = 999999999)
        {
            var rdm = new Random();
            int num = 0;
            do
            {
                num = rdm.Next(20, max);
            } while (records.Contains(num));
            return num;
        }




    }

    public static class RandomGenerator
    {
        /// <summary>
        /// Generate a random 10 digit number
        /// </summary>
        /// <returns></returns>
        public static string GenerateNumber()
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }
    }





    //public static class BarCodeUtilities
    //{
    //    public static byte[] GenerateBarcode(string str)
    //    {
    //        Barcode barcode = new Barcode();

    //        //Show the string
    //        barcode.IncludeLabel = true;
    //        barcode.Alignment = AlignmentPositions.CENTER;

    //        Image img = barcode.Encode(TYPE.CODE128, str, Color.Black, Color.White, 150, 40);

    //        return ImageUtilities.ImageToByteArray(img);
    //    }
    //}

}