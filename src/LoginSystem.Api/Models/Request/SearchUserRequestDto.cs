using FluentValidation;

namespace LoginSystem.Api.Models.Request;

public class SearchUserRequestDto
{
    public required string UserId { get; set; }
}


public class SearchUserRequestDtoValidation : AbstractValidator<SearchUserRequestDto>
{
    public SearchUserRequestDtoValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .NotNull()
            .Matches(@"^\d{0,7}$")
            .WithMessage("{PropertyName} must not be empty, less than 8 characters and convertible to integer.");
    }
}
