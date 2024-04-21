using System.Net;
using System.Net.Mail;

namespace LostButFound.API.Services.Helpers
{
    public class EmailSender
    {
        public void SendEmailCode(string adress, string code, bool isCode)
        {

            MailAddress fromAddress = new MailAddress("dvoryanchikov.danil@bk.ru", "AQA WEB PSB" +
                "");
            MailAddress toAddress = new MailAddress(adress);
            MailMessage message = new MailMessage(fromAddress, toAddress);
            if (isCode)
            {
                message.Body = code;
                message.Subject = "Ваш код подтверждения";
            }
            else
            {
                message.Body = $"<a href='{code}'>Восстановить пароль</a> ";
                message.Subject = "Восстановление пароля";
                message.IsBodyHtml = true;
            }
            

            using (SmtpClient smtp = new SmtpClient("smtp.bk.ru", 2525))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromAddress.Address, "Ht9L0gNwhr3QRxkyjcJf");
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
        }
    }
}
