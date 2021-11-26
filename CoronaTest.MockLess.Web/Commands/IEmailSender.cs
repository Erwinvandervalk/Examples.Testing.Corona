namespace CoronaTest.MockLess.Web.Commands
{
    public interface IEmailSender
    {
        void SendReminderEmail(string to, string title, string body);
    }
}