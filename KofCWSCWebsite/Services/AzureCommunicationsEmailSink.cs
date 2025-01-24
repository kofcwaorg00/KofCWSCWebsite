using Azure;
using Azure.Communication.Email;
using Serilog.Core;
using Serilog.Events;

namespace KofCWSCWebsite.Services
{
    public class AzureCommunicationEmailSink : ILogEventSink
    {
        private readonly EmailClient _emailClient;
        private readonly string _fromEmail;
        private readonly string _toEmail;

        public AzureCommunicationEmailSink(string connectionString, string fromEmail, string toEmail)
        {
            _emailClient = new EmailClient(connectionString);
            _fromEmail = fromEmail;
            _toEmail = toEmail;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level < LogEventLevel.Error)
                return;

            var message = new EmailMessage(
                senderAddress: _fromEmail,
                recipientAddress: _toEmail,
                content: new EmailContent("Error in Your Application")
                {
                    Html = logEvent.RenderMessage()
                }
            );

            try
            {
                _emailClient.Send(WaitUntil.Completed, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
