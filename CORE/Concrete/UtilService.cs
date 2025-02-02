using CORE.Abstract;
using CORE.Enums;
using ENTITIES.Enums;
using Microsoft.AspNetCore.Hosting;

namespace CORE.Concrete;

public class UtilService(IWebHostEnvironment environment) : IUtilService
{
    public string GetFolderName(EFileType type)
    {
        return type switch
        {
            EFileType.UserImage => @"files\user_images",
            _ => @"files\error"
        };
    }

    public string GetEnvFolderPath(string folderName)
    {
        return Path.Combine(environment.WebRootPath, folderName);
    }
}