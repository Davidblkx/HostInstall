

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Hosts_Install
{
    public class UserManager
    {
        public string OfflineFolder { get; }
        public const string FileName = "users.xml";
        public const string Domain = "https://raw.githubusercontent.com/Davidblkx/UserHosts/master/";

        public List<User> Users { get; }

        public UserManager(string offlineFolder)
        {
            OfflineFolder = offlineFolder;
            Users = new List<User>();
            ReloadUserList();
        }

        public void ReloadUserList()
        {
            Program.SAY("Downloading user list...");

            //Try to load user list from server
            try
            {
                var client = new WebClient {Encoding = Encoding.UTF8};
                var data = client.DownloadString(Domain + FileName);
                Program.SAY("Download completed");
                File.WriteAllText(Path.Combine(OfflineFolder, FileName), data, Encoding.UTF8);
                Program.SAY($"Saved to: {Path.Combine(OfflineFolder, FileName)}");
            }
            catch //Use offline backup if failed
            {
                Program.SAY($"Error loading user list, the offline backup will be used");
            }

            
            ParseUserList();
        }

        private void ParseUserList()
        {
            Users.Clear();
            var doc = XDocument.Parse(File.ReadAllText(Path.Combine(OfflineFolder, FileName)));

            var userNodes = doc.Descendants("user");
            foreach (var user in userNodes.Select(elem => new User(OfflineFolder)
            {
                Accounts = elem.Descendants("account").Select(x=>x.Value).ToList(),
                HostFiles = elem.Descendants("link").Select(x=>x.Value).ToList(),
                Name = elem.Attribute("name")?.Value
            }))
            {
                Users.Add(user);
            }
        }

        public User GetUser(string name)
        {
            return Users.FirstOrDefault(z => z.HasAccount(name)) ??
                   Users.FirstOrDefault(z => z.HasAccount("alunos"))
                   ?? Users.FirstOrDefault();
        }
    }
}
