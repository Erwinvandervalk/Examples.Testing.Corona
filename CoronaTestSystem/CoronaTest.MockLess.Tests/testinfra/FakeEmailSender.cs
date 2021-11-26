using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoronaTest.MockLess.Web.Commands;

namespace CoronaTest.MockLess.Tests.testinfra
{
    public class FakeEmailSender : IEmailSender
    {
        public List<(string To, string Title, string Body)> SentEmails =
            new List<(string To, string Title, string Body)>();

        public void SendReminderEmail(string to, string title, string body)
        {
            SentEmails.Add((to, title, body));
        }
    }
}
