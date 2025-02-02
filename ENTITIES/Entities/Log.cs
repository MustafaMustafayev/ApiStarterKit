using ENTITIES.Entities.Generic;
using ENTITIES.Enums;

namespace ENTITIES.Entities;

public class Log : IEntity
{
    public Guid Id { get; set; }
    public required DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;
    public required string AccessToken { get; set; }
    public required Guid? UserId { get; set; }
    public required string Path { get; set; }
    public required string Ip { get; set; }
    public required string Text { get; set; }
    public string? Description { get; set; }
    public required ELogType Type { get; set; }
    public required string StackTrace { get; set; }
    public bool IsDeleted { get; set; }
}