using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Hosts_Install
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Imports
    {
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void ShowConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_SHOW);
        }
        public static void HideConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        public static void ViewConsole(bool visible)
        {
            if(visible)
                ShowConsole();
            else
                HideConsole();
        }
    }
}
