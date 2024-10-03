using FluentValidation;
using LoginSystem.Domain.Models.Enum;

namespace LoginSystem.Api.Models.Request;

public class UpdateRegistrationStatusRequest
{
    public int UserId { get; set; }

    public UserStatus UserStatus { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateClosed { get; set; }

    public string? UpdatedBy { get; set; }
}

public class UpdateRegistrationStatusRequestValidator : AbstractValidator<UpdateRegistrationStatusRequest>
{
    public UpdateRegistrationStatusRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.UserStatus)
            .NotEmpty()
            .NotNull();
    }
}
