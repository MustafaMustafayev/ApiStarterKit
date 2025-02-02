using ENTITIES.Entities.Generic;
using System.ComponentModel.DataAnnotations;

namespace ENTITIES.Entities;

public class User : Auditable, IEntity
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    [EmailAddress] public required string Email { get; set; }
    [Phone] public required string ContactNumber { get; set; }
    public required string Password { get; set; }
    public required string Salt { get; set; }
    public string? Image { get; set; }
    public string? LastVerificationCode { get; set; }
}