namespace CORE.Constants;

public static class Constants
{
    public const string CORS_POLICY_NAME = "CorsPolicy";
    public const string HEALTH_CHECKS_URL = "/healthz";
    public const string HEALTH_CHECKS_UI_URL = "/health-ui";
    public const string MINI_PROFILER_URL = "/profiler";

    public static readonly string[] AllowedFileExtensions =
        [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpeg", ".jpg", ".png"];

    public static readonly string[] AllowedImageExtensions = [".jpeg", ".jpg", ".png"];
}