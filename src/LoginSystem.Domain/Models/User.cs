using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LoginSystem.Domain.Models.Enum;

namespace LoginSystem.Domain.Models;

public class User
{
    public Guid Id { get; set; }

    [Column(TypeName = "int")]
    [Required]
    public int UserId { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    [Required]
    public string UserName { get; set; }

    [Column(TypeName = "VARBINARY(max)")]
    [Required]
    public string Password { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    [Required]
    public string EmailAddress { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    [Required]
    public string MobileNumber { get; set; }

    [Column(TypeName = "varchar(10)")]
    [Required]
    public UserStatus UserStatus { get; set; }

    [Column(TypeName = "datetime")]
    [Required]
    public DateTime DateCreated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateClosed { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdated { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? UpdatedBy { get; set; }
}
