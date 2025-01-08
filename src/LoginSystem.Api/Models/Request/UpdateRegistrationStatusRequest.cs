using FluentValidation;
using LoginSystem.Api.Constants;
using LoginSystem.Domain.Models.Enum;

namespace LoginSystem.Api.Models.Request;

public class UpdateRegistrationStatusRequest
{
    public string Email { get; set; }

    public string UserName { get; set; }

    public UserStatus UserStatus { get; set; }

    public string Password { get; set; }

    public string? UpdatedBy { get; set; }
}

public class UpdateRegistrationStatusRequestValidator : AbstractValidator<UpdateRegistrationStatusRequest>
{
    public UpdateRegistrationStatusRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().NotNull();

        RuleFor(x => x.UserStatus)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .Matches(ValidationConstants.PasswordFormatting)
            .WithMessage("{PropertyName} must be not empty or must be NIST standards");
    }
}
