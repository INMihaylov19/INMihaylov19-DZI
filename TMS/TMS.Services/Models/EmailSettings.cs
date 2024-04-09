using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Services.Models
{
    public class EmailSettings
    {
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public string? Login { get; set; }
        public string? SmtpKey { get; set; }
        public bool EnableSsl { get; set; }
    }
}
