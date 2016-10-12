using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailTest
{
    interface IMailService
    {
        void SendMail();
        void SendMailAsync();
    }
}
