using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoronaTest.MockLess.Web.Commands
{
    public class EmailSender : IEmailSender
    {
        private readonly HttpClient _client;

        public EmailSender(HttpClient client)
        {
            _client = client;
        }

        public async void SendReminderEmail(string to, string title, string body)
        {
            var result = await _client.PostAsync("https://the-emailing-system", new StringContent("this is where the body would go"));

            // This is not the best way to do this but good enough for the demo
            result.EnsureSuccessStatusCode();
        }
    }
}
