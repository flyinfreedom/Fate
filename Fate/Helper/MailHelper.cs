using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Fate.Helper
{
    public class MailHelper : IDisposable
    {
        private SmtpClient smtpClient;
        private MailMessage mailMessage;

        private string _smtpClient = WebConfigVariable.smtpClient;
        private int _smtpPort = WebConfigVariable.smtpPort;
        private string _smtpUserName = WebConfigVariable.smtpUserName;
        private string _smtpPassword = WebConfigVariable.smtpPassword;
        private string _smtpDisplayName = WebConfigVariable.smtpDisplayName;
        private bool _smtpEnableSsl = WebConfigVariable.smtpEnableSsl;

        /// <summary>
        /// 建立SMTP資訊
        /// </summary>
        public MailHelper()
        {
            this.smtpClient = new SmtpClient(_smtpClient, _smtpPort);
            this.smtpClient.Credentials = new System.Net.NetworkCredential(_smtpUserName, _smtpPassword);
            this.smtpClient.EnableSsl = _smtpEnableSsl;
            this.mailMessage = new MailMessage();
        }

        /// <summary>
        /// 設定Mail的基本資訊
        /// </summary>
        /// <param name="ToList"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        public void SetMailMessage(string[] ToList, string Subject, string Body)
        {
            this.mailMessage.From = new MailAddress(_smtpUserName, _smtpDisplayName);
            foreach (string item in ToList)
            {
                this.mailMessage.To.Add(item);
            }
            this.mailMessage.Priority = MailPriority.Normal;
            this.mailMessage.Subject = Subject;
            this.mailMessage.Body = Body;
            this.mailMessage.IsBodyHtml = true;
        }

        /// <summary>
        /// 增加附件
        /// </summary>
        /// <param name="From"></param>
        /// <param name="DisplayName"></param>
        /// <param name="ToList"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="FilePath"></param>
        public void AttachFile(string FilePath)
        {
            Attachment data = new Attachment(FilePath, MediaTypeNames.Application.Octet);
            this.mailMessage.Attachments.Add(data);
        }

        public string AttachImg(string FilePath)
        {
            string ContentId = Guid.NewGuid().ToString();
            Attachment data = new Attachment(FilePath, MediaTypeNames.Application.Octet);
            this.mailMessage.Attachments.Add(data);
            this.mailMessage.Attachments[0].ContentId = ContentId;
            this.mailMessage.Attachments[0].ContentDisposition.Inline = true;
            this.mailMessage.Attachments[0].NameEncoding = Encoding.UTF8;
            this.mailMessage.SubjectEncoding = Encoding.UTF8;
            this.mailMessage.BodyEncoding = Encoding.UTF8;

            return ContentId;
        }

        /// <summary>
        /// 寄出Email
        /// </summary>
        public void Send()
        {
            this.smtpClient.Send(this.mailMessage);
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            this.smtpClient.Dispose();
            this.mailMessage.Dispose();
        }
    }
}