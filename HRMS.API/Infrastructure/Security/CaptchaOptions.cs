namespace HRMS.API.Infrastructure.Security;

public class CaptchaOptions
{
    public bool EnableInDevelopment { get; set; } = false;
    public int MaxFailedAttempts { get; set; } = 1;
    public int FailedAttemptsWindowMinutes { get; set; } = 5;
}

