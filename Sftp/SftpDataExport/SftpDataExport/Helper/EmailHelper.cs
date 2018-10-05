using SftpDataExport.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SftpDataExport.Helper
{
    public class EmailHelper
    {
        public static string GetEmailContent(string path, Dictionary<string, string> replacements)
        {
            string content = File.ReadAllText(path);
            foreach (var kvp in replacements)
            {
                content = content.Replace(kvp.Key, kvp.Value);
            }
            return content;
        }

        public static bool SendEmail(string mailFrom, string mailTo, string subject, string body, List<string> attachments = null)
        {
            if (string.IsNullOrEmpty(mailFrom))
                throw new ArgumentNullException("mailFrom");

            if (string.IsNullOrEmpty(mailFrom))
                throw new ArgumentNullException("mailTo");

            return SendEmail(new MailAddress(mailFrom), new MailAddress(mailTo), subject, body, attachments);
        }

        public static bool SendEmail(MailAddress mailFrom, MailAddress mailTo, string subject, string body, List<string> attachments = null)
        {
            return SendEmail(mailFrom, new MailAddressCollection { mailTo }, null, subject, body, attachments);
        }

        public static bool SendEmail(string mailFrom, string mailTo, List<string> mailCcList, string subject, string body, List<string> attachments = null)
        {
            if (string.IsNullOrEmpty(mailFrom))
                throw new ArgumentNullException("mailFrom");

            if (string.IsNullOrEmpty(mailFrom))
                throw new ArgumentNullException("mailTo");

            var _mailFrom = new MailAddress(mailFrom);
            var _mailToList = new MailAddressCollection { new MailAddress(mailTo) };
            var _mailCcList = new MailAddressCollection();

            if (mailCcList != null)
            {
                foreach (string mail in mailCcList)
                {
                    _mailCcList.Add(new MailAddress(mail));
                }
            }

            return SendEmail(_mailFrom, _mailToList, _mailCcList, subject, body, attachments);
        }

        public static bool SendEmail(MailAddress mailFrom, List<MailAddress> mailToList, List<MailAddress> mailCcList, string subject, string body, List<string> attachments = null)
        {
            var _mailToList = new MailAddressCollection();
            var _mailCcList = new MailAddressCollection();

            if (mailToList != null)
            {
                foreach (MailAddress ma in mailToList)
                {
                    _mailToList.Add(ma);
                }
            }

            if (_mailCcList != null)
            {
                foreach (MailAddress ma in mailCcList)
                {
                    _mailCcList.Add(ma);
                }
            }

            return SendEmail(mailFrom, _mailToList, _mailCcList, subject, body, attachments);
        }

        public static bool SendEmail(MailAddress mailFrom, MailAddressCollection mailToList, MailAddressCollection mailCcList, string subject, string body, List<string> attachments)
        {
            try
            {
                var mail = new MailMessage
                {
                    From = mailFrom,
                    Sender = mailFrom,
                    Subject = subject,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = body
                };

                if (mailToList != null)
                {
                    foreach (MailAddress mailTo in mailToList)
                    {
                        mail.To.Add(mailTo);
                    }
                }

                if (mailCcList != null)
                {
                    foreach (MailAddress mailCc in mailCcList)
                    {
                        mail.CC.Add(mailCc);
                    }
                }

                if (attachments != null)
                {
                    foreach (string attachment in attachments)
                    {
                        // Create the file attachment for this e-mail message.
                        var data = new Attachment(attachment, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(attachment);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachment);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(attachment);
                        // Add the file attachment to this e-mail message.
                        mail.Attachments.Add(data);
                    }
                }

                SmtpClient client = new SmtpClient();

                if (!string.IsNullOrEmpty(AppSettings.SmtpServer))
                {
                    client.Host = AppSettings.SmtpServer;
                    client.Port = AppSettings.SmtpPort;
                    client.EnableSsl = AppSettings.SmtpEnableSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    if (AppSettings.SmtpEnableSsl && !string.IsNullOrEmpty(AppSettings.SmtpAccount))
                    {
                        client.Credentials = new NetworkCredential(AppSettings.SmtpAccount, AppSettings.SmtpPassword);
                    }
                }
                else
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                }

                client.Send(mail);

                mail.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                string toList = mailToList == null ? "null" : mailToList.ToString();
                string ccList = mailCcList == null ? "null" : mailCcList.ToString();
                Log.Error($@"Exception Thrown sending email, from: {mailFrom} to: {toList}, cc: {ccList}, subject: {subject}, body: {body}", ex);
                return false;
            }
        }
    }
}
