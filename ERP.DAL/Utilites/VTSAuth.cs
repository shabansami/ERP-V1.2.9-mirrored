using ERP.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ERP.DAL.Utilites
{

    public class VTSAuth
    {
        readonly string cookiename = "_erp";
        readonly int cookieKeyCount = 1;
        public static readonly int cookieHours = 12;
        public static bool IsDemo { get; set; } 
        public UserInfo CookieValues { get; set; }
        public string CodePrefix => "o";//code prefix sell invoices Online  
        //public List<SideBarPages> SideBarPages { get; set; }
        //private Guid SessionID { get; set; }


        //constant crypto variables 
        private readonly static string passPhrase = "Pa7Xpgz@sE";     // can be any string
        private readonly static string saltValue = "sapkrzVAluE";     // can be any string
        private readonly static string hashAlgorithm = "SHA1";     // can be "MD5"
        private readonly static int passwordIterations = 5;     // can be any number
        private readonly static string initVector = "@1B8c3Y4x0F1w7H9";     // must be 16 bytes
        private readonly static int keySize = 256;     // can be 192 or 128

        public static string Encrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }
        public static string Decrypt(string cipherText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length - 1 + 1];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            return plainText;
        }
        public static string CreateMD5(string input) //MD5 Hashing
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public void adminPass()
        {
            var t = Encrypt("P@ss@rd");
        }
        /// <summary>
        /// check if the cookies has values and the values count is equal to or bigger than the cookie keys count
        /// </summary>
        public bool CheckCookies() => HttpContext.Current.Request.Cookies[cookiename] != null && HttpContext.Current.Request.Cookies[cookiename].Values.Count >= cookieKeyCount;
        //public bool CheckSidBarCookies() => HttpContext.Current.Request.Cookies["_pkr1"] != null && HttpContext.Current.Request.Cookies["_pkr1"].Values.Count >= cookieKeyCount;
        /// <summary>
        /// Load Data saved in the cookies
        /// </summary>
        /// <returns>true if data existed in the cookies</returns>
        public bool LoadDataFromCookies()
        {
            if (CheckCookies())
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
                CookieValues = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(Decrypt(cookie.Values[0]));
                return true;
            }
            return false;
        }

        //public bool LoadDataSidBarFromCookies()
        //{
        //    if (CheckSidBarCookies())
        //    {
        //        HttpCookie cookie = HttpContext.Current.Request.Cookies["_pkr1"];
        //        SideBarPages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SideBarPages>>(Decrypt(cookie.Values[0]));
        //        return true;
        //    }
        //    return false;
        //}


        public void ClearCookies()
        {
            if (HttpContext.Current.Request.Cookies[cookiename] != null)
            {
                HttpContext.Current.Response.Cookies[cookiename].Expires = DALUtility.GetDateTime().AddDays(-1);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SaveToCookies(UserInfo cookieValues)
        {
            if (cookieValues != null)
            {
                HttpCookie cookie = new HttpCookie(cookiename);
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cookieValues);
                cookie.Values.Add("k0", Encrypt(jsonString.ToString()));
                cookie.Expires = DALUtility.GetDateTime().AddHours(cookieHours);
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            return false;
        }
        //internal bool SaveSidBarToCookies(List<SideBarPages> cookieValues)
        //{
        //    if (cookieValues != null)
        //    {
        //        HttpCookie cookie = new HttpCookie("_pkr1");
        //        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cookieValues);
        //        cookie.Values.Add("k1", Encrypt(jsonString.ToString()));
        //        cookie.Expires = Utility.GetDateTime().AddDays(cookieDays);
        //        HttpContext.Current.Response.Cookies.Add(cookie);
        //        return true;
        //    }
        //    return false;
        //}
    }

   
   
}
