using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Hosts_Install
{
    public class User
    {
        public User(string offlineFolder)
        {
            OfflinePath = offlineFolder;
            Accounts = new List<string>();
            HostFiles = new List<string>();
        }

        public string Name { get; set; }
        public List<string> Accounts { get; set; }
        public List<string> HostFiles { get; set; }
        public string OnlineUri { get; } = "https://raw.githubusercontent.com/Davidblkx/UserHosts/master/";
        public string OfflinePath { get; }

        public bool DownloadHosts()
        {
            var sucess = true;
            Program.SAY($"Handling Hosts file for: {Name}");
            Program.SAY($"Preparing to download {HostFiles.Count} files");

            foreach (var host in HostFiles)
            {
                try
                {
                    var client = new WebClient();
                    var webUri = new Uri(OnlineUri + host);
                    var offHost = Path.Combine(OfflinePath, host);

                    Program.SAY("===========================================================");
                    Program.SAY($"Downloading from {webUri}...");

                    string data = client.DownloadString(webUri);
                    File.WriteAllText(offHost, data);

                    Program.SAY("Download completed");
                    Program.SAY($"Saved to: {offHost}");
                    Program.SAY("===========================================================\n");
                }
                catch
                {
                    Program.SAY("Download failed", ConsoleColor.Red);
                    Program.SAY("===========================================================\n");
                    sucess = false;
                }
            }

            return sucess;
        }

        public string BuildHostsFile()
        {
            var hostsContent = "";

            foreach (var hosts in HostFiles)
            {
                var filePath = Path.Combine(OfflinePath, hosts);
                if (!File.Exists(filePath)) continue;

                var data = File.ReadAllText(filePath);

                hostsContent += $"#{hosts}\n{data}\n";
            }

            return hostsContent;
        }

        public bool HasAccount(string name)
        {
            return Accounts.Count(z => string.Equals(z, name, StringComparison.CurrentCultureIgnoreCase)) > 0;
        }
    }
}