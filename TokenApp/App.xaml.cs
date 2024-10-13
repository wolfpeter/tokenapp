using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace TokenApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Mutex mutex = null;

    protected override void OnStartup(StartupEventArgs e)
    {
        const string mutexName = "MyUniqueAppName";

        // Ellenőrizzük, hogy van-e már egy példány futásban
        bool isNewInstance;
        mutex = new Mutex(true, mutexName, out isNewInstance);

        if (!isNewInstance)
        {
            MessageBox.Show("Az alkalmazás már fut!");
            Application.Current.Shutdown();  // Az új példány bezárása
            return;
        }

        base.OnStartup(e);
    }
}