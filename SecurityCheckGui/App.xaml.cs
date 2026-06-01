using System.Runtime.InteropServices;
using System.Windows;

namespace SecurityCheckGui;

public partial class App : Application
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_HIDE = 0;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Verstecke das Konsolen-Fenster
        IntPtr consoleWindow = GetConsoleWindow();
        if (consoleWindow != IntPtr.Zero)
        {
            ShowWindow(consoleWindow, SW_HIDE);
        }

        System.Diagnostics.Debug.WriteLine("App startup");
    }
}
