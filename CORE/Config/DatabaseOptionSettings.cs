namespace CORE.Config;

public record DatabaseOptionSettings
{
    public byte MaxRetryCount { get; set; }
    public byte CommandTimeout { get; set; }
    public bool EnableDetailedErrors { get; set; }
}