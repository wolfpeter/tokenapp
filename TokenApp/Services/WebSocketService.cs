using System.Net.WebSockets;
using System.Text;

namespace TokenApp.Services;

public class WebSocketService
{
    private ClientWebSocket _clientWebSocket;
    
    public WebSocketService()
    {
        _clientWebSocket = new ClientWebSocket();
    }

    public async Task<bool> ConnectWebSocketAsync(string socketUri, CancellationToken cancellationToken = default)
    {
        try
        {
            await _clientWebSocket
                .ConnectAsync(new Uri("ws://" + socketUri + ":5059/Peripherals/POSDesktopApplication"), cancellationToken);
            
            return true;
        }
        catch (Exception ex)
        {
            
            MessageBox.Show($"Hiba: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendRegistrationMessageAsync()
    {
        try
        {
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hiba: {ex.Message}");
            return false;
        }
    }
}