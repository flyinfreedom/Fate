using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Fate.Helper
{
    public class WebConfigVariable
    {
        private static string _fateTypeCookieName = "FateTypeCookies";


        private static string _smtpClient = WebConfigurationManager.AppSettings["smtpClient"];
        private static string _smtpPort = WebConfigurationManager.AppSettings["smtpPort"];
        private static string _smtpUserName = WebConfigurationManager.AppSettings["smtpUserName"];
        private static string _smtpPassword = WebConfigurationManager.AppSettings["smtpPassword"];
        private static string _smtpDisplayName = WebConfigurationManager.AppSettings["smtpDisplayName"];
        private static string _smtpEnableSsl = WebConfigurationManager.AppSettings["smtpEnableSsl"];

        private static string _ipTablePath = WebConfigurationManager.AppSettings["ipTablePath"];


        #region Property
        public static string FateTypeCookieName
        {
            get { return _fateTypeCookieName; }
        }

        public static string smtpClient
        {
            get { return _smtpClient; }
        }
        public static int smtpPort
        {
            get { return Convert.ToInt32(_smtpPort); }
        }
        public static string smtpUserName
        {
            get { return _smtpUserName; }
        }
        public static string smtpPassword
        {
            get { return _smtpPassword; }
        }
        public static string smtpDisplayName
        {
            get { return _smtpDisplayName; }
        }
        public static bool smtpEnableSsl
        {
            get { return Convert.ToBoolean(_smtpEnableSsl); }
        }

        public static string ipTablePath
        {
            get { return _ipTablePath; }
        }

        public static string GetTxIdUrl { get { return WebConfigurationManager.AppSettings["GetTxIdUrl"]; } }
        public static string PaymentUrl { get { return WebConfigurationManager.AppSettings["PaymentUrl"]; } }
        public static string QueryTxIdStatusUrl { get { return WebConfigurationManager.AppSettings["QueryTxIdStatusUrl"]; } }
        public static string PaymentCallBackUrl { get { return WebConfigurationManager.AppSettings["PaymentCallBackUrl"]; } }
        public static string CID { get { return WebConfigurationManager.AppSettings["CID"]; } }
        public static string BaseUrl { get { return WebConfigurationManager.AppSettings["BaseUrl"]; } }        
        #endregion
    }
}