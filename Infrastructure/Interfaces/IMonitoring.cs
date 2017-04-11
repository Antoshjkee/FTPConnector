using FluentFTP;
using Newtonsoft.Json.Linq;

namespace FTPConnector.Infrastructure.Interfaces
{
    public interface IMonitoring
    {
        void Start(int periodMin, FtpClient client, JArray fileStructure);
    }
}
