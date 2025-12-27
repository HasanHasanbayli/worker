namespace Worker.Application.Services;

public interface ISmsService
{
    // Sends an SMS message to the specified phone number.
    // Your implementation may vary based on the SMS provider you choose.

    public Task SendSmsAsync();
}