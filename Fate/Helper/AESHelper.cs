using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Fate.Helper
{
    public static class AESHelper
    {
        private const string _key = "A5HA6F1MDMJ5207VLFL38XJ26RJ47AAV";
        private const string _iv = "ONNQ18IOSJBB2HH2";

        private const string ZiweiKey = "914DWEWUBXAZCUS0GO5F73WLZCS7G7KX";
        private const string ZiweiIv = "IBGYAQ9O1XNUVUTN";
        private const string ST01Key = "6UY3YIQUVNYEO1H5TV7EXXAJL2V6T81S";
        private const string ST01Iv = "1OI82GUKI39K2631";
        private const string NA01Key = "7AM5TWMWD0B53S62P9N6872M9CABCQ7E";
        private const string NA01Iv = "O649NX0FSRSHV3BH";

        public static string Encrypt(string toEncrypt, string productId)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(_key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(_iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            switch (productId.ToUpper())
            {
                case "ZIWEI":
                    keyArray = UTF8Encoding.UTF8.GetBytes(ZiweiKey);
                    ivArray = UTF8Encoding.UTF8.GetBytes(ZiweiIv);
                    break;
                case "ST01":
                    keyArray = UTF8Encoding.UTF8.GetBytes(ST01Key);
                    ivArray = UTF8Encoding.UTF8.GetBytes(ST01Iv);
                    break;
                case "NA01":
                    keyArray = UTF8Encoding.UTF8.GetBytes(NA01Key);
                    ivArray = UTF8Encoding.UTF8.GetBytes(NA01Iv);
                    break;
            }

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
 
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
 
        public static string Decrypt(string toDecrypt, string productId)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(_key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(_iv);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            switch (productId.ToUpper())
            {
                case "ZIWEI":
                    keyArray = UTF8Encoding.UTF8.GetBytes(ZiweiKey);
                    ivArray = UTF8Encoding.UTF8.GetBytes(ZiweiIv);
                    break;
                case "ST01":
                    keyArray = UTF8Encoding.UTF8.GetBytes(ST01Key);
                    ivArray = UTF8Encoding.UTF8.GetBytes(ST01Iv);
                    break;
                case "NA01":
                    keyArray = UTF8Encoding.UTF8.GetBytes(NA01Key);
                    ivArray = UTF8Encoding.UTF8.GetBytes(NA01Iv);
                    break;
            }

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
 
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
 
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}