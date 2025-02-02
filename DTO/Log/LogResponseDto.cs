using ENTITIES.Enums;

namespace DTO.Log;

public record LogResponseDto
{
    public Guid Id { get; set; }
    public required string DateTime { get; set; }
    public string? AccessToken { get; set; }
    public Guid? UserId { get; set; }
    public string? Path { get; set; }
    public string? Ip { get; set; }
    public required string Text { get; set; }
    public string? Description { get; set; }
    public required ELogType Type { get; set; }
    public string? StackTrace { get; set; }
}