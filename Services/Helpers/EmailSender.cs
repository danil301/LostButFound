using System.Net;
using System.Net.Mail;

namespace LostButFound.API.Services.Helpers
{
    public class EmailSender
    {
        public void SendEmailCode(string adress, string code)
        {
            MailAddress fromAddress = new MailAddress("lostbutfoundcomp@gmail.com", "LostButFound" +
                "");
            MailAddress toAddress = new MailAddress(adress);
            MailMessage message = new MailMessage(fromAddress, toAddress);
            message.Body = code;
            message.Subject = "Ваш код подтверждения";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(fromAddress.Address, "trzn hyeg loxh ugoe");

            smtpClient.Send(message);
        }
    }
}
