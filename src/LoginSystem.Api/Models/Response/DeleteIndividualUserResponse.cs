using LoginSystem.Domain.Models.Enum;

namespace LoginSystem.Api.Models.Response;

public class DeleteIndividualUserResponse
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public string EmailAddress { get; set; }

    public string MobileNumber { get; set; }

    public UserStatus UserStatus { get; set; }

    public DateTime DateClosed { get; set; }

    public string UpdatedBy { get; set; }
}
