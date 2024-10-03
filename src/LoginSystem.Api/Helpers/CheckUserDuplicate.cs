namespace LoginSystem.Api.Helpers;

public class CheckUserDuplicate
{
    public bool IsDuplicate { get; set; }
    public required string ErrorCode { get; set; }
}
