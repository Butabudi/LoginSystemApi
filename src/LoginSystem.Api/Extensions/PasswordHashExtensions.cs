namespace LoginSystem.Api.Extensions;

public class PasswordHashExtensions
{
    public static string PasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }
}
