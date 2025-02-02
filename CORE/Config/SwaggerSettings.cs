namespace CORE.Config;

public record SwaggerSettings
{
    public bool IsEnabled { get; set; }
    public required string Title { get; set; }
    public required string Version { get; set; }
    public required string Theme { get; set; }
}