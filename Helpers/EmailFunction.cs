using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace tufol.Helpers
{
    public class EmailFunction
    {
        private IWebHostEnvironment _iHostingEnvironment;
        private String _current_language;
        public EmailFunction()
        {
            _current_language = Thread.CurrentThread.CurrentCulture.Name;
        }
        public async Task<string> GenerateEmailTemplate(string template_path)
        {
            String result;

            using (StreamReader streamReader = System.IO.File.OpenText(template_path))
            {
                result = await streamReader.ReadToEndAsync();
            }
            return result;
        }
        public String GenerateEmailContentFromTemplate(string template_content, Dictionary<string, string> content_param)
        {
            foreach (var entry in content_param)
            {
                template_content = Regex.Replace(template_content, @Regex.Escape("[[" +entry.Key+"]]"), entry.Value);
            }
            return template_content;
        }

        public List<String> GetEmailList(string email, string separator = ";")
        {
            List<String> result = new List<String>();
            try
            {
                if ( !string.IsNullOrEmpty(email) )
                {
                    if ( email.Contains(";") )
                    {
                        string[] split_email = email.Split(separator);
                        foreach (var email_item in split_email)
                        {
                            if (!String.IsNullOrEmpty(email_item))
                                result.Add(email_item);
                        }
                    } else
                    {
                        result.Add(email);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return result;
        }
        public void sending_email(string to, string subject, string message)
        {
            String from = "Dita_Nurhalimah@berca.co.id";
            var smtp = new SmtpClient
            {
                Host = "webmail.berca.co.id",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(from, "Dita_Nurhalimah.290421")

            };
            try
            {
                var senderEmail = new MailAddress(from);
                var receiverEmail = new MailAddress(to, "Receiver");
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(mess);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
