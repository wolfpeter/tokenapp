using System.ComponentModel;
using System.Windows.Input;
using TokenApp.Helpers;
using TokenApp.Services;

namespace TokenApp.ViewModels;

public class MainWindowsViewModel : INotifyPropertyChanged
{
    public ICommand GenerateTokenCommand { get; }
    
    private readonly TokenService tokenService;
    private readonly ApiService apiService;
    private readonly MainService mainService;
    private readonly WebSocketService webSocketService;
    
    private string apiBaseUrl;
    private string email;
    private string alias;
    private string token;
    private bool configured;
    private bool loggedIn;
    private string selectedBlockPrinter;
    private string selectedLanguage;

    public MainWindowsViewModel()
    {
        Alias = Properties.Settings.Default.Alias;
        LoggedIn = Properties.Settings.Default.LoggedIn;
        Configured = Properties.Settings.Default.Configured;
        ApiBaseUrl = Properties.Settings.Default.ApiBaseUrl;
        SelectedBlockPrinter = Properties.Settings.Default.SelectedBlockPrinter;
        SelectedLanguage = Properties.Settings.Default.SelectedLanguage;
        
        tokenService = new TokenService();
        apiService = new ApiService();
        mainService = new MainService();
        webSocketService = new WebSocketService();
        
        GenerateTokenCommand = new RelayCommand(GenerateToken);

        DeviceId = mainService.GenerateDeviceId();
    }
    
    public string ApiBaseUrl
    {
        get { return apiBaseUrl; }
        set
        {
            if (apiBaseUrl != value)
            {
                apiBaseUrl = value;
                
                Properties.Settings.Default.ApiBaseUrl = apiBaseUrl;
                Properties.Settings.Default.Save();
            }
        }
    }
    
    public string Email
    {
        get { return email; }
        set
        {
            if (email != value)
            {
                email = value;
            }
        }
    }
    
    public string Alias
    {
        get { return alias; }
        set
        {
            if (alias != value)
            {
                alias = value;
                OnPropertyChanged(nameof(Alias));
                
                Properties.Settings.Default.Alias = alias;
                Properties.Settings.Default.Save();
            }
        }
    }

    public string Token
    {
        get { return token; }
        private set
        {
            token = value;
            OnPropertyChanged(nameof(Token));
        }
    }
    
    public bool Configured
    {
        get { return configured; }
        set
        {
            if (configured != value)
            {
                configured = value;
                
                Properties.Settings.Default.Configured = configured;
                Properties.Settings.Default.Save();
            }
        }
    }
    
    public bool LoggedIn
    {
        get { return loggedIn; }
        set
        {
            if (loggedIn != value)
            {
                loggedIn = value;
                
                Properties.Settings.Default.LoggedIn = loggedIn;
                Properties.Settings.Default.Save();
            }
        }
    }
    
    public string DeviceId { get; }

    public string SelectedBlockPrinter
    {
        get { return selectedBlockPrinter; }
        set
        {
            if (selectedBlockPrinter != value)
            {
                selectedBlockPrinter = value;
                
                Properties.Settings.Default.SelectedBlockPrinter = selectedBlockPrinter;
                Properties.Settings.Default.Save();
                
                OnPropertyChanged(nameof(SelectedBlockPrinter));
            }
        }
    }
    
    public string SelectedLanguage
    {
        get { return selectedLanguage; }
        set
        {
            if (selectedLanguage != value)
            {
                selectedLanguage = value;
                
                Properties.Settings.Default.SelectedLanguage = selectedLanguage;
                Properties.Settings.Default.Save();
                
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }
    }
    
    public async Task<bool> ConnectToWebSocket(CancellationToken cancellationToken)
    {
        if (ApiBaseUrl == "test" && Alias == "test")
        {
            await Task.Delay(2000, cancellationToken);
            return true;
        }
        
        return await webSocketService.ConnectWebSocketAsync(ApiBaseUrl, cancellationToken);
    }
    
    public async Task<bool> SendRegistrationToWebSocket()
    {
        if (ApiBaseUrl == "test" && Alias == "test") return true;
        
        return await webSocketService.SendRegistrationMessageAsync();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void GenerateToken()
    {
        Token = tokenService.GenerateToken(Alias);
    }

    public async Task<bool> Login(string password)
    {
        if (ApiBaseUrl == "test" && Email == "test" && password == "test")
        {
            return true;
        }
        
        return await apiService.Login(ApiBaseUrl, Email, password);
    }

    public async Task<bool> LoginWithSecondaryPassword(string password)
    {
        if (ApiBaseUrl == "test" && Email == "test" && password == "test")
        {
            return true;
        }
        
        return await apiService.LoginWithSecondaryPassword(password);
    }
}