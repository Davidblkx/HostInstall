using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;

namespace Hosts_Install
{
    public class Program
    {
        public static string LastSay = (new DateTime()).ToString(CultureInfo.CurrentCulture);
        public static string OfflineFolderName { get; } = "\\Hosts";
        public static string AppName { get; } = "Hosts.exe";
        private static string OfflineFolder { get; set; }
        private static string AppLocation { get; set; }
        private static string LogFile { get; set; }
        private static string HostsFile { get; set; }

        private static void Main(string[] args)
        {
            try
            {
                BuildVars();

                SayHello();

                if (File.Exists(LogFile))
                    if ((new FileInfo(LogFile)).Length >= 50000)
                        File.Delete(LogFile);

                if (args.Length == 0)
                {
                    Imports.HideConsole();
                    DefaultStart();
                    return;
                }

                if (args.Contains("-menu")) MenuStart();
            }
            catch (Exception error)
            {
                SAY(error.Message);
            }
            finally
            {
                SayBye();
            }
        }

        private static void DefaultStart()
        {
            var userName = Environment.UserName;
            SAY($"Searching hosts config for: {userName}");
            var user = (new UserManager(OfflineFolder)).GetUser(userName);
            SAY($"Set hosts config for: {user.Name}");
            File.WriteAllText(HostsFile, user.BuildHostsFile());
        }

        private static void MenuStart()
        {
            Console.Clear();
            Console.WriteLine("============================");
            Console.WriteLine("===== UserHosts Config =====");
            Console.WriteLine("============================");
            Console.WriteLine(" 1 - Install");
            Console.WriteLine(" 2 - Uninstall");
            Console.WriteLine(" x - Exit");
            Console.WriteLine("============================");

            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Install();
                    break;

                case ConsoleKey.D2:
                    Uninstall();
                    break;

                case ConsoleKey.X:
                    return;

                default:
                    Console.WriteLine("Invalid key...");
                    Console.ReadKey(true);
                    MenuStart();
                    break;
            }
        }

        private static void Install()
        {
            Console.Clear();
            var winVersion = OsTools.GetOsVersion();
            var appName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName);
            var currentAppLocal = Path.Combine(Environment.CurrentDirectory, appName + ".exe");
            var count = 0;
            var error = 0;

            SAY($"Runing install for: {winVersion.Name}");

            if(AppLocation != currentAppLocal)
                File.Copy(currentAppLocal, AppLocation, true);

            //set bat to startup
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key?.SetValue("Hosts", AppLocation);

            SAY("Set startup file at:");
            SAY(AppLocation);
            

            //Create dirs
            if (!Directory.Exists(OfflineFolder))
                Directory.CreateDirectory(OfflineFolder);
            SAY($"Set as offline folder at: {OfflineFolder}");

            //Imports users
            SAY("Downloading users...");
            var userManager = new UserManager(OfflineFolder);
            foreach (var user in userManager.Users)
            {
                if (!user.DownloadHosts())
                    error++;
                count++;
            }

            SAY($"Downnload completed, success: {count - error} of {count}");

            //Set folders and files permissions
            SetDirectoryPermissions(OfflineFolder, FileSystemRights.FullControl);
            SAY("Fullcontrol set to Offline folder");
            SetFilePermissions(HostsFile, FileSystemRights.FullControl);
            
            SAY("Press any key to exit...");
            Console.ReadKey();
        }

        private static void Uninstall()
        {
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key?.DeleteValue("Hosts", false);
            SAY("Removed from startup");

            SAY("Sucessfully uninstalled");
            SAY("Press any key to exit...");
            Console.ReadKey();
        }

        private static void BuildVars()
        {
            var drive = Environment.GetEnvironmentVariable("SYSTEMDRIVE");
            var root = Environment.GetEnvironmentVariable("SYSTEMROOT");
            OfflineFolder = (drive ?? "C:") + OfflineFolderName;
            AppLocation = Path.Combine(OfflineFolder, AppName);
            LogFile = Path.Combine(OfflineFolder, "APP.LOG");
            HostsFile = (root ?? @"c:\windows") + @"\system32\drivers\etc\hosts";
        }

        private static void SetDirectoryPermissions(string dirPath, FileSystemRights fileRights)
        {
            var info = new DirectoryInfo(dirPath);
            var acl = info.GetAccessControl();
            var userId = (new SecurityIdentifier(WellKnownSidType.WorldSid, null)).Translate(typeof (NTAccount));
            acl.AddAccessRule(new FileSystemAccessRule(userId, fileRights, AccessControlType.Allow));
            Directory.SetAccessControl(dirPath, acl);
        }
        private static void SetFilePermissions(string filePath, FileSystemRights fileRights)
        {
            var info = new FileInfo(filePath);
            var acl = info.GetAccessControl();
            var userId = (new SecurityIdentifier(WellKnownSidType.WorldSid, null)).Translate(typeof(NTAccount));
            acl.AddAccessRule(new FileSystemAccessRule(userId, fileRights, AccessControlType.Allow));
            File.SetAccessControl(filePath, acl);
        }

        // ReSharper disable once InconsistentNaming
        public static void SAY(string message, ConsoleColor txtColor = ConsoleColor.White)
        {
            Console.ForegroundColor = txtColor;
            if (DateTime.Now.ToString(CultureInfo.CurrentCulture) == LastSay)
                File.AppendAllText(LogFile, "\n" + message);
            else
            {
                LastSay = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                File.AppendAllText(LogFile, $"\n#{LastSay}:\n" + message);
            }
            Console.WriteLine(message);
        }

        private static void SayHello()
        {
            SAY(LastSay);
            SAY("==================== STARTING ========================");
            Console.Clear();

            
        }
        private static void SayBye()
        {
            SAY("===================== STOPING ========================");
            Console.Clear();
        }
    }
}