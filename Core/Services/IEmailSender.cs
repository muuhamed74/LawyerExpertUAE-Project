using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string Emailto, string subject, string body, IList<IFormFile> attatchments = null);
    }
}
