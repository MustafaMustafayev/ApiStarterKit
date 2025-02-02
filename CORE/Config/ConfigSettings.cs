namespace CORE.Config;

public record ConfigSettings
{
    public AuthSettings AuthSettings { get; set; } = null!;
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
    public DatabaseOptionSettings DatabaseOptionSettings { get; set; } = null!;
    public RequestSettings RequestSettings { get; set; } = null!;
    public SwaggerSettings SwaggerSettings { get; set; } = null!;
    public CryptographySettings CryptographySettings { get; set; } = null!;
}