namespace LoginSystem.Api.Constants;

public static class ValidationConstants
{
    public const string PasswordFormatting = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";

    public const string MobileFormatting = @"^(09|\+639)\d{9}$";
}
