using Worker.Application.Ports;

namespace Worker.Infrastructure.Services;

public class EmailService : IEmailService
{
    // Sends an email to the specified recipient.
    // Your implementation may vary based on the email provider you choose.

    public Task SendEmailAsync()
    {
        // Placeholder for email sending logic
        return Task.CompletedTask;
    }
}