using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace FTPConnector.Hubs
{
    [HubName("FileMonitoringHub")]
    public class FileMonitoringHub : Hub
    {
        public void Send(string message)
        {
            Clients.All.BroadcastMessage(message);
        }
    }
}
