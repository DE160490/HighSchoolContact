using System.Net;
using System.Net.Mail;

namespace FBT.Filter
{
    public class MailUtils
    {
        public static string SendGmail(string _from, string _to, string _subject, string _body, string _gmail, string _password)
        {
            MailMessage msg = new MailMessage(_from, _to, _subject, _body);
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.ReplyToList.Add(new MailAddress(_from));
            msg.Sender = new MailAddress(_to);

            using var smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_gmail, _password);

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
