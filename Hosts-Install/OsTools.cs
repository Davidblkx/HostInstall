using System;
using System.IO;

namespace Hosts_Install
{
    public static class OsTools
    {
        public static OsVersion GetOsVersion()
        {
            var os = new OsVersion
            {
                HostsFile = Path.Combine(Environment.SystemDirectory, @"\Drivers\etc\hosts"),
            };

            var winVersion = Environment.OSVersion;

            switch (winVersion.Version.Major)
            {
                case 5:
                    os.StartUpFile = Environment.GetEnvironmentVariable("AllUsersProfile");
                    os.StartUpFile += @"\Start Menu\Programs\Startup\SetUserHost.bat";
                    os.Name = winVersion.Version.Minor == 1 ? "Windows XP" : "Windows 2000";
                    break;

                case 6:
                    os.StartUpFile = Environment.GetEnvironmentVariable("AllUsersProfile");
                    os.StartUpFile += @"\Microsoft\Windows\Start Menu\Programs\StartUp\SetUserHost.bat";

                    switch (winVersion.Version.Minor)
                    {
                        case 0:
                            os.Name = "Windows Vista";
                            break;
                        case 1:
                            os.Name = "Windows 7";
                            break;
                        case 2:
                            os.Name = "Windows 8";
                            break;
                        case 3:
                            os.Name = "Windows 8.1";
                            break;
                    }

                    break;
            }

            return os;
        }
    }

    public class OsVersion
    {
        public string Name { get; set; }
        public string HostsFile { get; set; }
        public string StartUpFile { get; set; }
    }
}
