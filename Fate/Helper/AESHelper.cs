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
        private const string _key = "8E02183B322FFAEAE692AC9D754B7FD1";
        private const string _iv = "19F502BD7134CBBB";
        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(_key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(_iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
 
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
 
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
 
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
 
        public static string Decrypt(string toDecrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(_key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(_iv);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
 
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