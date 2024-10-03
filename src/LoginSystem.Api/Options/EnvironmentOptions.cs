using System.ComponentModel.DataAnnotations;

namespace LoginSystem.Api.Options;

public class EnvironmentOptions
{
    [Required]
    public required string Name { get; set; }
}
