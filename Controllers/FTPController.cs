using FluentFTP;
using FTPConnector.Infrastructure;
using FTPConnector.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace FTPConnector.Controllers
{
    public class FTPController : Controller
    {
        private IFtpDirectorySearch<JArray> _directorySearch;
        private IMonitoring _monitoring;

        public FTPController(IFtpDirectorySearch<JArray> directorySearch, IMonitoring monitoring)
        {
            _directorySearch = directorySearch;
            _monitoring = monitoring;
        }

        [HttpGet("/api/connection/{host}/{user}/{password}")]
        public IActionResult CheckConnection(string host, string user, string password)
        {
            try
            {
                var client = new FtpClient
                {
                    Host = host,
                    Credentials = new NetworkCredential(user, password)
                };

                client.Connect();

                var fileStructure = _directorySearch.GetRecursiveFilesJson(client, "/");

                _monitoring.Start(1 ,client, fileStructure);

                return Ok(fileStructure);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error occured: {ex.Message}");
            }
        }
    }
}

