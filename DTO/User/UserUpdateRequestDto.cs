using System.ComponentModel.DataAnnotations;

namespace DTO.User;

public record UserUpdateRequestDto
{
    [EmailAddress] public required string Email { get; set; }
    [Phone] public required string ContactNumber { get; set; }
    public required string Username { get; set; }
}