using CORE.Enums;
using ENTITIES.Enums;

namespace CORE.Abstract;

public interface IUtilService
{
    public string GetFolderName(EFileType type);
    string GetEnvFolderPath(string folderName);
}