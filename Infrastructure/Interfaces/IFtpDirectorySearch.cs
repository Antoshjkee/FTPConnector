using FluentFTP;

namespace FTPConnector.Infrastructure
{
    public interface IFtpDirectorySearch<T>
    {
        T GetRecursiveFilesJson(FtpClient client, string path);
        T GetIterativeFilesJson(FtpClient client, string path);
        T CompareDirectories(T source, T target);
    }
}
