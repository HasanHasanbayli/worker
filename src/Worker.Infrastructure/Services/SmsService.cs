using Worker.Application.Services;

namespace Worker.Infrastructure.Services;

public class SmsService : ISmsService
{
    // Sends an SMS message to the specified phone number.
    // Your implementation may vary based on the SMS provider you choose.

    public Task SendSmsAsync()
    {
        // Placeholder for SMS sending logic
        return Task.CompletedTask;
    }
}