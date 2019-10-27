using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Fate.Backoffice.Helper
{
    public static class SHA256Helper
    {
        public static string Encoding(string inputString)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(inputString);
            data = new SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.UTF8.GetString(data);
        }
    }
}