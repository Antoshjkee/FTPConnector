using FluentFTP;
using FTPConnector.Hubs;
using FTPConnector.Infrastructure.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using WebApplicationBasic;

namespace FTPConnector.Infrastructure
{
    public class Monitoring : IMonitoring
    {
        private Timer _timer;
        private IFtpDirectorySearch<JArray> _directorySearch;

        public Monitoring(IFtpDirectorySearch<JArray> directorySearch)
        {
            _directorySearch = directorySearch;
        }
        public void Start(int periodMin, FtpClient client, JArray fileStructure)
        {
            var context = Startup.ConnectionManager.GetHubContext<FileMonitoringHub>();

            context.Clients.All.BroadCastMessage($"Monitoring started at {DateTime.Now}");

            _timer = new Timer((e) =>
            {
                fileStructure = _directorySearch.CompareDirectories(fileStructure, _directorySearch.GetRecursiveFilesJson(client, "/"));
            }, null, 0, (int)TimeSpan.FromMinutes(periodMin).TotalMilliseconds);
        }
    }
}
