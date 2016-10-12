using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace MailTest
{
    public class SMTPMailService : IMailService
    {
        private readonly string smtp_server = ConfigurationManager.AppSettings["smtp_server"].ToString().Trim();
        private readonly string smtp_uid = ConfigurationManager.AppSettings["smtp_uid"].ToString().Trim();
        private readonly string smtp_pwd = ConfigurationManager.AppSettings["smtp_pwd"].ToString().Trim();
        private readonly string mail_type = ConfigurationManager.AppSettings["mail_type"].ToString().Trim();
        private readonly string mailImgPath = ConfigurationManager.AppSettings["mailImgPath"].ToString().Trim();

        /// <summary>
        /// 发送邮件
        /// 如果需要接收异常（可以使用Try Catch包含调用此方法的代码）
        /// </summary>
        public void SendMail()
        {
            using (SmtpClient smptClient = new SmtpClient())
            {
                smptClient.Host = smtp_server;
                // 通过本地IIS SMTP服务器发送邮件到邮件服务器
                smptClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                //SMTP 服务器要求安全连接需要设置此属性
                smptClient.EnableSsl = false;
                if (!mail_type.Equals("0"))
                    smptClient.UseDefaultCredentials = true;
                else
                    smptClient.Credentials = new System.Net.NetworkCredential(smtp_uid, smtp_pwd);

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtp_uid);
                    // 收件人
                    mailMessage.To.Add(new MailAddress("xxxxxxx@xxx.com"));
                    // 抄送(可以添加多个)
                    //mailMessage.CC.Add(new MailAddress(""));
                    // 秘密抄送
                    //mailMessage.Bcc.Add(new MailAddress(""));
                    // 正文编码
                    mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    // 附件(可以多个)
                    mailMessage.Attachments.Add(new Attachment("img/mail_header.png"));
                    mailMessage.Attachments[0].ContentId = "mail_header";
                    mailMessage.Attachments[0].ContentDisposition.Inline = true;//作为内嵌元素
                    mailMessage.Attachments.Add(new Attachment("img/mail_footer.png"));
                    mailMessage.Attachments[1].ContentId = "mail_footer";
                    mailMessage.Attachments[1].ContentDisposition.Inline = true;//作为内嵌元素
                                                                                // 邮件主题
                    mailMessage.Subject = "This is test";
                    // 正文
                    mailMessage.Body = GetMailBody().Replace("@@mail_header@@", "cid:" + mailMessage.Attachments[0].ContentId).Replace("@@mail_footer@@", "cid:" + mailMessage.Attachments[1].ContentId);
                    mailMessage.Body = mailMessage.Body.Replace("@@Title@@", "尊敬的Jiawei(此段内容可被替换):").Replace("@@Content@@", "欢迎你参加XXX技术等级考试，如有其它疑问请联系xxxxxxx@xxx.com(此段内容也可被替换)");
                    // Html格式
                    mailMessage.IsBodyHtml = true;

                    smptClient.Send(mailMessage);
                }
            }
        }

        public void SendMailAsync()
        {
            throw new NotImplementedException();
        }

        private string GetMailBody()
        {
            string body = @"<!DOCTYPE html>
                            <!--[if IE 8]>    <html class='no-js ie8 ie' lang='en'> <![endif]-->
                            <!--[if IE 9]>    <html class='no-js ie9 ie' lang='en'> <![endif]-->
                            <!--[if gt IE 9]><!-->
                            <html class='no-js' lang='en'>
                            <!--<![endif]-->
                            <head>
                                <title>Build your email | Campaign Monitor</title>
                                <meta charset='utf-8'>
                                <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'>
                            </head>
                            <body>
                                <div style='background-color: #fff;' class='wrapper'>
                                    <table align='center' style='border-collapse: collapse;table-layout: fixed;Margin-left: auto;Margin-right: auto;overflow-wrap: break-word;word-wrap: break-word;word-break:inherit;background-color: #ffffff;' class='layout layout--no-gutter'>
                                        <tbody>
                                            <tr>
                                                <td width='600' style='padding: 0;text-align: left;vertical-align: top;color: #8f8f8f;font-size: 14px;line-height: 21px;font-family: &quot;Open Sans&quot;,sans-serif;' class='column'>
                                                    <div align='center' style='font-size: 12px;font-style: normal;font-weight: 400;' class='image'>
                                                        <img height='123' width='600' alt='' src='@@mail_header@@' style='display: block;border: 0;max-width: 690px;' class='gnd-corner-image gnd-corner-image-center gnd-corner-image-top'>
                                                    </div>
                                                    <div style='Margin-left: 20px;Margin-right: 20px;Margin-top: 20px;'>
                                                        <div style='display: block;font-size: 2px;line-height: 2px;width: 40px;background-color: #ccc;Margin-left: 260px;Margin-right: 260px;Margin-bottom: 20px;' class='divider'>&nbsp;</div>
                                                    </div>
                                                    <div style='Margin-left: 20px;Margin-right: 20px;'>
                                                        <div style='line-height:10px;font-size:1px'>&nbsp;</div>
                                                    </div>
                                                    <div style='Margin-left: 20px;Margin-right: 20px;'>
                                                        <h2 style='Margin-top: 0;Margin-bottom: 0;font-style: normal;font-weight: normal;color: #706f70;font-size: 22px;line-height: 31px;font-family: Cabin,Avenir,sans-serif;' class='size-22'><strong>@@Title@@</strong></h2><p style='Margin-top: 16px;Margin-bottom: 20px;font-size: 16px;line-height: 24px;text-align: left;' class='size-16'><span>@@Content@@&nbsp;</span></p>
                                                    </div>
                                                    <div style='Margin-left: 20px;Margin-right: 20px;'>
                                                        <div style='display: block;font-size: 2px;line-height: 2px;width: 40px;background-color: #ccc;Margin-left: 260px;Margin-right: 260px;Margin-bottom: 20px;' class='divider'>&nbsp;</div>
                                                    </div>
                                                    <div style='Margin-left: 20px;Margin-right: 20px;'>
                                                        <div style='line-height:20px;font-size:1px'>&nbsp;</div>
                                                    </div>
                                                    <div align='center' style='font-size: 12px;font-style: normal;font-weight: 400;' class='image'>
                                                        <img height='43' width='522' alt='' src='@@mail_footer@@' style='display: block;border: 0;max-width: 522px;'>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
        
                                    <img border='0' height='1' width='1' alt='' src='https://none.createsend1.com/t/i-o-hluruyy-l/o.gif' style='border: 0 !important;visibility: hidden !important;display: block !important;height: 1px !important;width: 1px !important;margin: 0 !important;padding: 0 !important;'>
                                </div>
                            </body>
                            </html>";
            return body;
        }
    }
}