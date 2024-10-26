using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TokenApp.Services;
using TokenApp.ViewModels;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;

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
        LoadPrinters();
        LoadSavedBlockPrinter();
        
        ChangeLanguage(_mainViewModel.SelectedLanguage);
        SetSelectedLanguage(_mainViewModel.SelectedLanguage);
        
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
            SwitchToLoginPage(null, null);
        }
        else
        {
            SwitchToApiRootPage();
        }
    }
    
    private void CheckPasswordAndCallMethod(object sender, EventArgs e)
    {
        MasterPasswordWindow passwordDialog = new MasterPasswordWindow();
        if (passwordDialog.ShowDialog() == true)
        {
            string enteredPassword = passwordDialog.Password;
            string correctPassword = "test";
            
            if (enteredPassword == correctPassword)
            {
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

    private void BlockPrinterEditButton_Click(object sender, RoutedEventArgs e)
    {
        BlockPrinterNameTxt.Visibility = Visibility.Collapsed;
        BlockPrinterNameComboBox.Visibility = Visibility.Visible;
    }

    private void BlockPrinterSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selectedPrinter = BlockPrinterNameComboBox.SelectedItem.ToString();

        if (!string.IsNullOrEmpty(selectedPrinter))
        {
            _mainViewModel.SelectedBlockPrinter = selectedPrinter;
            
            BlockPrinterNameTxt.Visibility = Visibility.Visible;
            BlockPrinterNameComboBox.Visibility = Visibility.Collapsed;
        }
    }
    
    private void LoadPrinters()
    {
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            BlockPrinterNameComboBox.Items.Add(printer);
        }
    }

    private void ChangeLanguage(string langCode)
    {
        ResourceDictionary dict = new ResourceDictionary();
        switch (langCode)
        {
            case "en":
                dict.Source = new Uri("Resources/Strings.en.xaml", UriKind.Relative);
                break;
            default:
                dict.Source = new Uri("Resources/Strings.hu.xaml", UriKind.Relative);
                break;
        }

        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(dict);
        
        _mainViewModel.SelectedLanguage = langCode;
    }

    private void SetLanguageToEnglish(object sender, EventArgs e)
    {
        ChangeLanguage("en");
        SetSelectedLanguage("en");
    }
    
    private void SetLanguageToHungarian(object sender, EventArgs e)
    {
        ChangeLanguage("hu");
        SetSelectedLanguage("hu");
    }
    
    private void OnLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedLanguage = (sender as ComboBox).SelectedItem as ComboBoxItem;
        string langCode = selectedLanguage.Tag.ToString();
    
        ChangeLanguage(langCode);
    }
    
    private void SetSelectedLanguage(string langCode)
    {
        if (langCode == "en")
        {
            HungarianLang.IsChecked = false;
            EnglishLang.IsChecked = true;
        } else if (langCode == "hu")
        {
            EnglishLang.IsChecked = false;
            HungarianLang.IsChecked = true;
        }
    }

    private void ApiRootPageNext(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_mainViewModel.ApiBaseUrl))
        {
            SwitchToAliasPage();
        }
    }
    
    private async void LoginPageNext(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_mainViewModel.Email) || !string.IsNullOrEmpty(txtPassword.Password))
        {
            LoginPage.IsEnabled = false;
            bool result = await _mainViewModel.Login(txtPassword.Password);
            LoginPage.IsEnabled = true;
            
            if (result)
            {
                SwitchToSecondaryPasswordPage();
            }
            else
            {
                SwitchToLoginPage(sender, e);
            }
        }
    }
    
    private async void SecondaryPasswordPageNext(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtSecondaryPassword.Password))
        {
            SecondPasswordPage.IsEnabled = false;
            bool result = await _mainViewModel.LoginWithSecondaryPassword(txtSecondaryPassword.Password);
            SecondPasswordPage.IsEnabled = true;
            
            if (result)
            {
                _mainViewModel.LoggedIn = true;
                SwitchToSettings(sender, e);
            }
            else
            {
                SwitchToSecondaryPasswordPage();
            }
        }
    }
    
    private void AliasPageNext(object sender, RoutedEventArgs e)
    {
        SwitchToLoginPage(sender, e);
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
    
    private void SwitchToLoginPage(object sender, RoutedEventArgs e)
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
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void PrintTest(object sender, RoutedEventArgs e)
    {
        PrintService printService = new PrintService();
        printService.PrintDocument(_mainViewModel.SelectedBlockPrinter);
    }
    
    private void LoadSavedBlockPrinter()
    {
        string savedPrinter = Properties.Settings.Default.SelectedBlockPrinter;
        if (!string.IsNullOrEmpty(savedPrinter) && BlockPrinterNameComboBox.Items.Contains(savedPrinter))
        {
            BlockPrinterNameComboBox.SelectedItem = savedPrinter;
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