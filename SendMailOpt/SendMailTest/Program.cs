using MailTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMailTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SMTPMailService service = new SMTPMailService();
            service.SendMail();
            Console.WriteLine("发送成功!");
            Console.Read();
        }
    }
}
