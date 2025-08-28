using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Identity;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Serv.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly EmailSetting _mailsetting;

        public EmailSender(IConfiguration config, IOptions<EmailSetting> mailsetting)
        {
            _config = config;
            _mailsetting = mailsetting.Value;
        }





        public async Task SendEmailAsync(string Emailto, string subject, string body, IList<IFormFile> attatchments = null)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailsetting.Email),
                Subject = subject
            };
            email.To.Add(MailboxAddress.Parse(Emailto));

            var builder = new BodyBuilder();

            if (attatchments != null && attatchments.Count > 0)
            {
                byte[] fileBytes;
                foreach (var attachment in attatchments)
                {
                    using var stream = new MemoryStream();
                    await attachment.CopyToAsync(stream);
                    builder.Attachments.Add(attachment.FileName, stream.ToArray());
                }
            }

            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailsetting.Displayname, _mailsetting.Email));

            using var smtp = new SmtpClient();

            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;


            smtp.Connect(_mailsetting.Host, _mailsetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailsetting.Email, _mailsetting.Password);
            await smtp.SendAsync(email);

            smtp.Disconnect(true);


        }
    }
}
