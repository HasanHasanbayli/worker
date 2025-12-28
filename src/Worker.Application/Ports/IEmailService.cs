namespace Worker.Application.Ports;

public interface IEmailService
{
    // Sends an email to the specified recipient.
    // Your implementation may vary based on the email provider you choose.

    public Task SendEmailAsync();
}