using System.ComponentModel.DataAnnotations;
using FluentValidation;
using LoginSystem.Api.Constants;

namespace LoginSystem.Api.Models.Request;

public class CreateUserRequestDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string MobileNumber { get; set; }
}

public class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserRequestDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().NotNull();
        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .Matches(ValidationConstants.PasswordFormatting)
            .WithMessage("{PropertyName} must be not empty or must be NIST standards");
        RuleFor(x => x.MobileNumber).NotEmpty().NotNull().Matches(ValidationConstants.MobileFormatting);
    }
}
