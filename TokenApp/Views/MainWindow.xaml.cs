using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TokenApp.Services;
using TokenApp.ViewModels;
using Button = System.Windows.Controls.Button;

namespace TokenApp.Views;

public partial class MainWindow : Window
{
    private NotifyIcon notifyIcon;
    private DispatcherTimer _timer;
    private int _timeLeft;

    private readonly MainWindowsViewModel _mainViewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        _mainViewModel = new MainWindowsViewModel();
        DataContext = _mainViewModel;
        
        StartTokenTimer();
        CreateTrayIcon();
        
        Start();
    }

    private void Start()
    {
        CloseAllGridsVisibility();

        if (_mainViewModel.LoggedIn && _mainViewModel.Configured)
        {
            SwitchToMainPage(null, null);
        }
        else if (_mainViewModel.LoggedIn && !_mainViewModel.Configured)
        {
            SwitchToSettings(null, null);
        }
        else if (!_mainViewModel.LoggedIn && !string.IsNullOrEmpty(_mainViewModel.ApiBaseUrl))
        {
            SwitchToLoginPage();
        }
        else
        {
            SwitchToApiRootPage();
        }
    }
    
    private void CheckPasswordAndCallMethod(object sender, EventArgs e)
    {
        // Jelszó bekérése
        MasterPasswordWindow passwordDialog = new MasterPasswordWindow();
        if (passwordDialog.ShowDialog() == true)  // Ha az OK gombra kattintott
        {
            string enteredPassword = passwordDialog.Password;
            string correctPassword = "mySecretPassword";  // Az elvárt jelszó
            
            // Jelszó ellenőrzése
            if (enteredPassword == correctPassword)
            {
                // Hívjuk meg a kívánt metódust
                ResetSettings(null, null);
            }
        }
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
        e.Cancel = true;
        Hide();
    }

    private void StartTokenTimer()
    {
        _timeLeft = 6000;
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
        _mainViewModel.GenerateToken();
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

    private void ApiRootPageNext(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_mainViewModel.ApiBaseUrl))
        {
            SwitchToLoginPage();
        }
    }
    
    private async void LoginPageNext(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_mainViewModel.Email) || !string.IsNullOrEmpty(txtPassword.Password))
        {
            bool result = await _mainViewModel.Login(txtPassword.Password);
            
            if (result)
            {
                SwitchToSecondaryPasswordPage();
            }
            else
            {
                SwitchToLoginPage();
            }
        }
    }
    
    private async void SecondaryPasswordPageNext(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtSecondaryPassword.Password))
        {
            bool result = await _mainViewModel.LoginWithSecondaryPassword(txtSecondaryPassword.Password);
            
            if (result)
            {
                _mainViewModel.LoggedIn = true;
                SwitchToAliasPage();
            }
            else
            {
                SwitchToSecondaryPasswordPage();
            }
        }
    }
    
    private void AliasPageNext(object sender, RoutedEventArgs e)
    {
        SwitchToSettings(sender, e);
        _mainViewModel.Configured = true;
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
        txtRootUrl.Clear();
        ApiRootPage.Visibility = Visibility.Visible;
        txtRootUrl.Focus();
    }
    
    private void SwitchToLoginPage()
    {
        CloseAllGridsVisibility();
        txtEmail.Clear();
        txtPassword.Clear();
        LoginPage.Visibility = Visibility.Visible;
        txtEmail.Focus();
    }
    
    private void SwitchToSecondaryPasswordPage()
    {
        CloseAllGridsVisibility();
        txtSecondaryPassword.Clear();
        SecondPasswordPage.Visibility = Visibility.Visible;
        txtSecondaryPassword.Focus();
    }
    
    private void SwitchToAliasPage()
    {
        CloseAllGridsVisibility();
        txtAlias.Clear();
        AliasPage.Visibility = Visibility.Visible;
        txtAlias.Focus();
    }

    private void Logout(object sender, RoutedEventArgs e)
    {
        txtEmail.Clear();
        txtPassword.Clear();
        txtSecondaryPassword.Clear();
        txtAlias.Clear();
        
        _mainViewModel.LoggedIn = false;
        
        Start();
    }

    private void ResetSettings(object sender, RoutedEventArgs e)
    {
        txtRootUrl.Clear();
        txtEmail.Clear();
        txtPassword.Clear();
        txtSecondaryPassword.Clear();
        txtAlias.Clear();
        
        _mainViewModel.LoggedIn = false;
        _mainViewModel.Configured = false;
        
        Start();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button != null && button.ContextMenu != null)
        {
            // Biztosítsuk, hogy a ContextMenu leváljon a régi helyről
            button.ContextMenu.PlacementTarget = button;
        
            // Állítsuk be, hogy a gombnál jelenjen meg a menü
            button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        
            // Nyissuk meg a menüt
            button.ContextMenu.IsOpen = true;
        }
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