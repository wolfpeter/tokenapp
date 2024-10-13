using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TokenApp.Views;

public partial class MainWindow : Window
{
    private NotifyIcon notifyIcon;
    private DispatcherTimer _timer;
    private int _timeLeft;
    private Random _random;

    public MainWindow()
    {
        InitializeComponent();
        _random = new Random();
        StartTokenTimer();
        
        CreateTrayIcon();
    }
    
    private void CreateTrayIcon()
    {
        notifyIcon = new NotifyIcon();
        notifyIcon.Icon = SystemIcons.Application; //TODO Icon file!
        notifyIcon.Visible = true;
        notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
    }
    
    private void NotifyIcon_DoubleClick(object sender, EventArgs e)
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }
    
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // Megakadályozza az ablak bezárását, csak minimalizáljuk
        e.Cancel = true;
        Hide();
    }

    private void StartTokenTimer()
    {
        _timeLeft = 6000; // 60 másodperc visszaszámlálás
        TokenProgressBar.Value = _timeLeft;

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(10);
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _timeLeft--;
        TokenProgressBar.Value = _timeLeft;

        if (_timeLeft <= 0)
        {
            GenerateNewToken();
        }
    }

    private void GenerateNewToken()
    {
        _timeLeft = 6000;
        string newToken = GenerateRandomToken();
        TokenCode.Text = newToken;
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        PrinterNameTxt.Visibility = Visibility.Collapsed;
        PrinterNameComboBox.Visibility = Visibility.Visible;
    }

    private void PrinterSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PrinterNameTxt.Visibility = Visibility.Visible;
        PrinterNameComboBox.Visibility = Visibility.Collapsed;
    }

    private string GenerateRandomToken()
    {
        // Generálj egy új token kódot, például 3 betű + 3 szám formátumban
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        string token = new string(new char[] {
            chars[_random.Next(chars.Length)],
            chars[_random.Next(chars.Length)],
            chars[_random.Next(chars.Length)],
            nums[_random.Next(nums.Length)],
            nums[_random.Next(nums.Length)],
            nums[_random.Next(nums.Length)]
        });
        return token;
    }

    private void ApiRootPageNext(object sender, RoutedEventArgs e)
    {
        SwitchToLoginPage();
    }
    
    private void LoginPageNext(object sender, RoutedEventArgs e)
    {
        SwitchToSecondaryPasswordPage();
    }
    
    private void SecondaryPasswordPageNext(object sender, RoutedEventArgs e)
    {
        SwitchToAliasPage();
    }
    
    private void AliasPageNext(object sender, RoutedEventArgs e)
    {
        SwitchToSettings(sender, e);
    }
    
    private void SwitchToSettings(object sender, RoutedEventArgs e)
    {
        CloseAllGridsVisibility();
        SettingsPage.Visibility = Visibility.Visible;
    }

    private void SwitchToMainPage(object sender, RoutedEventArgs e)
    {
        GenerateNewToken();
        
        CloseAllGridsVisibility();
        MainPage.Visibility = Visibility.Visible;
    }
    
    private void SwitchToApiRootPage()
    {
        CloseAllGridsVisibility();
        ApiRootPage.Visibility = Visibility.Visible;
    }
    
    private void SwitchToLoginPage()
    {
        CloseAllGridsVisibility();
        LoginPage.Visibility = Visibility.Visible;
    }
    
    private void SwitchToSecondaryPasswordPage()
    {
        CloseAllGridsVisibility();
        SecondPasswordPage.Visibility = Visibility.Visible;
    }
    
    private void SwitchToAliasPage()
    {
        CloseAllGridsVisibility();
        AliasPage.Visibility = Visibility.Visible;
    }
    
    private void CloseAllGridsVisibility()
    {
        foreach (var child in ParentGrid.Children)
        {
            if (child is Grid grid)
            {
                grid.Visibility = Visibility.Collapsed;
            }
        }
    }
}