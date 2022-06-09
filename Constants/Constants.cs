namespace nng.Constants;

public static class Constants
{
    public static readonly TimeSpan CaptchaBlockWaitTime = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan CaptchaEditorWaitTime = TimeSpan.FromHours(1);
    public static readonly TimeSpan TooManyRequestsWait = TimeSpan.FromSeconds(3);
}
