using FluentFTP;
using FTPConnector.Hubs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationBasic;

namespace FTPConnector.Infrastructure
{
    public class FtpDirectorySearch : IFtpDirectorySearch<JArray>
    {
        private ILogger<FtpDirectorySearch> _logger;

        public FtpDirectorySearch(ILogger<FtpDirectorySearch> logger)
        {
            _logger = logger;
        }
        public JArray GetRecursiveFilesJson(FtpClient client, string path)
        {
            var files = new JArray();

            try
            {
                foreach (FtpListItem item in client.GetListing(path))
                {
                    if (item.Type == FtpFileSystemObjectType.File)
                    {
                        var file = new JObject(
                            new JProperty("name", item.FullName),
                            new JProperty("size", item.Size),
                            new JProperty("modified", item.Modified.ToString("MM/dd/yy H:mm:ss")));

                        files.Add(file);

                    }
                    else
                    {
                        var folder = new JObject(
                        new JProperty("name", item.FullName),
                        new JProperty("modified", item.Modified.ToString("MM/dd/yy H:mm:ss"))) { { "files", GetRecursiveFilesJson(client, item.FullName) } };

                        files.Add(folder);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return files;
        }

        public JArray GetIterativeFilesJson(FtpClient client, string path)
        {
            var files = new JArray();
            var folderQueue = new Queue<FtpListItem>();

            try
            {
                // Adding root folder into queue
                folderQueue.Enqueue(new FtpListItem());

                while (folderQueue.Count != 0)
                {
                    var currentDirectory = folderQueue.Dequeue();
                    var currentFolderFiles = new JArray();

                    var folder = new JObject(
                        new JProperty("name", currentDirectory.Name == null ? "/" : currentDirectory.Name),
                        new JProperty("modified", currentDirectory.Modified == DateTime.MinValue ? client.GetModifiedTime(path) : currentDirectory.Modified),
                        new JProperty("files", String.Empty));

                    files.Add(folder);

                    foreach (FtpListItem item in client.GetListing(currentDirectory.FullName))
                    {

                        if (item.Type == FtpFileSystemObjectType.File)
                        {
                            var obj = new JObject(
                            new JProperty("name", item.Name),
                            new JProperty("size", item.Size),
                            new JProperty("modified", item.Modified));

                            currentFolderFiles.Add(obj);
                        }
                        else
                        {
                            folderQueue.Enqueue(item);
                        }

                    }

                    files.Last["files"] = currentFolderFiles;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return files;
        }

        public JArray CompareDirectories(JArray source, JArray target)
        {
            // Getting SignalR Hub Context
            var context = Startup.ConnectionManager.GetHubContext<FileMonitoringHub>();

            try
            {
                foreach (var sourceItem in source)
                {
                    var foundItem = target.FirstOrDefault(targetItem => targetItem["name"].ToString() == sourceItem["name"].ToString());
                    if (foundItem != null)
                    {
                        if (DateTime.Parse(foundItem["modified"].ToString()) != DateTime.Parse(sourceItem["modified"].ToString()))
                        {
                            context.Clients.All.BroadCastMessage($"Item {foundItem["name"]} has been modified");
                        }

                        if (foundItem["files"] != null)
                        {
                            CompareDirectories((JArray)foundItem["files"], (JArray)sourceItem["files"]);
                        }
                    }
                    else
                    {
                        context.Clients.All.BroadCastMessage($"Item {sourceItem["name"]} added");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return target;
        }
    }
}
