namespace Worker.Application.Shared.Constants;

public class CronExpression
{
    public const string DailyAt0200 = "0 2 * * *"; // At 02:00 AM every day
    public const string EveryHour = "0 * * * *"; // At minute 0 past every hour
}