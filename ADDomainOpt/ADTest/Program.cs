using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Web.UI;

namespace ADTest
{
    class Program
    {
        static void Main(string[] args)
        {
            GetNameAndMail();
        }
        static void IsAuthenticated()
        {
            // 输入测试
            string domain = "Code1";
            string name = "310227284";
            //string name = "666666";
            //string password = "Philips7";
            string password = "Wicresoft1";

            // 测试是否在域内
            LdapAuthentication ldap = new LdapAuthentication();
            //LdapAuthentication ldap = new LdapAuthentication("LDAP://wicresoft");
            bool isAuthenticated = ldap.IsAuthenticated(domain, name, password);
            Console.WriteLine(isAuthenticated);

            Console.Read();
        }
        static void GetNameAndMail()
        {
            // 输入测试
            string domain = "Code1";
            string name = "310227284";
            //string name = "666666";
            //string password = "Philips7";
            string password = "Wicresoft1";
            // 输出结果
            List<string> paths = new List<string>();


            // 获取输入
            Console.WriteLine("输入域名:");
            domain = Console.ReadLine();
            Console.WriteLine("输入用户名:");
            name = Console.ReadLine();
            Console.WriteLine("输入密码:");
            password = Console.ReadLine();

            // 测试获取用户名
            LdapAuthentication ldap = new LdapAuthentication();
            ADUserInfo user = new ADUserInfo();
            string res = ldap.GetADUserNameAndEmail(domain, name, password, out paths, out user);
            if (res.Equals("ok"))
            {
                Console.WriteLine("UserPrincipalName-->" + user.UserPrincipalName);
                Console.WriteLine("mail-->" + user.mail);
                Console.WriteLine("streetAddress-->" + user.streetAddress);
                Console.WriteLine("displayName-->" + user.displayName);
                Console.WriteLine("st-->"+user.st);
                Console.WriteLine("physicalDeliveryOfficeName-->" + user.physicalDeliveryOfficeName);
                Console.WriteLine("telephoneNumber-->" + user.telephoneNumber);
                Console.WriteLine("l-->"+user.l);
                Console.WriteLine("sn-->" + user.sn);
                Console.WriteLine("givenName-->" + user.givenName);
                Console.WriteLine("description-->" + user.description);
                Console.WriteLine("distinguishedName-->" + user.distinguishedName);
                Console.WriteLine("whenCreated-->" + user.whenCreated);
                Console.WriteLine("sAMAccountName-->" + user.sAMAccountName);
                Console.WriteLine("mailnickname-->" + user.mailnickname);
                Console.WriteLine("msexchhomeservername-->" + user.msexchhomeservername);
                Console.WriteLine("physicaldeliveryofficename-->" + user.physicaldeliveryofficename);
                Console.WriteLine("samaccountname-->" + user.samaccountname);
                Console.WriteLine("name-->" + user.name);
            }
            else
            {
                Console.WriteLine(res);
            }
            Console.Read();
        }
    }
}
