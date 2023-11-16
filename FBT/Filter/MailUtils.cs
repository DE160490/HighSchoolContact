using System.Net;
using System.Net.Mail;

namespace FBT.Filter
{
    public class MailUtils
    {
        public static string SendGmail(string _From, string _To, string _Subject, string _Body, string _Gmail, string _Password)
        {
            MailMessage msg = new MailMessage(_From, _To, _Subject, _Body);
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.ReplyToList.Add(new MailAddress(_From));
            msg.Sender = new MailAddress(_To);

            using var smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;

            // Sửa new NetworkCredential(_Gmail, "Mã gmail đăng nhập");
            smtpClient.Credentials = new NetworkCredential(_Gmail, "codegmail");

            try
            {
                smtpClient.Send(msg);
                return "Gửi gmail thành công";
            }
            catch (Exception ex)
            {
                return "Gửi gmail thất bại: " + ex.Message;
            }
        }
    }
}
