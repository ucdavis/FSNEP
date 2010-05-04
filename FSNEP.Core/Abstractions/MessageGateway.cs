using System.Net.Mail;

namespace FSNEP.Core.Abstractions
{
    public interface IMessageGateway
    {
        void SendMessage(string from, string to, string subject, string body);
    }

    public class MessageGateway : IMessageGateway
    {
        public void SendMessage(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);
            var client = new SmtpClient("smtp.ucdavis.edu");

            client.Send(message);
        }
    }
}