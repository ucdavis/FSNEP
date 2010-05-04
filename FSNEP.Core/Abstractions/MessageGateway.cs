using System;
using System.Net.Mail;
using System.Text;
using FSNEP.Core.Domain;
using System.Web.Configuration;

namespace FSNEP.Core.Abstractions
{
    public interface IMessageGateway
    {
        void SendMessage(string to, string subject, string body);
        void SendMessageToNewUser(User user, string username, string userEmail, string supervisorEmail);
    }

    public class MessageGateway : IMessageGateway
    {
        public void SendMessage(string to, string subject, string body)
        {
            string from = WebConfigurationManager.AppSettings["EmailFrom"];
            
            var message = new MailMessage(from, to, subject, body);
            var client = new SmtpClient("smtp.ucdavis.edu");

            client.Send(message);
        }

        /// <summary>
        /// Send the new user a message explaining how to login to their account
        /// </summary>
        public void SendMessageToNewUser(User user, string username, string userEmail, string supervisorEmail)
        {
            var newUserPath = new StringBuilder();
            newUserPath.Append("PATH WITH TOKEN");

            var body = new StringBuilder();

            body.AppendLine("Welcome to the FSNEP Time/Expense Record System!");
            body.AppendFormat("Please note that your UserID is {0}. We ask that you create a password to access the system by accessing this link:  {1}", username, newUserPath.ToString());
            body.AppendLine(Environment.NewLine);

            //Temporary User Contact Information
            body.AppendLine("During the November 2008, December 2008 and January 2009 of system implementation please contact your State Office FSNEP Representative for hands on assistance with the system.");
            body.AppendLine("Central Coast and Southern = Susan Padgett @ sdpadget@ucdavis.edu   530-754-4137");
            body.AppendLine("Central Valley Region = Corine Gould @ cgould@ucdavis.edu   530-754-4934");
            body.AppendLine("North Coast and Mountain = Gladis Lopez @ gylopez@ucdavis.edu  530-754-7796");
            body.Append(Environment.NewLine);

            body.AppendFormat("During standard system use following implementation (after February 1, 2009), please contact the FSNEP supervisor({0}) if you have any questions.", supervisorEmail);
            body.Append(Environment.NewLine);

            body.AppendLine(Environment.NewLine);

            body.Append("Welcome again to FSNEP.");
            body.AppendLine(Environment.NewLine);

            body.AppendLine("FSNEP");
            body.AppendLine("System Administrator");

            SendMessage(userEmail, "FSNEP Time/Expense Record System", body.ToString());
        }
    }
}