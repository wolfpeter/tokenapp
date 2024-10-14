using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace TokenApp.Services;

public class ApiService
{
    private HttpClient httpClient;
    private string apiUrl;
    private string? authToken1;
    private string? authToken2;

    public async Task<bool> Login(string apiUrl, string email, string password)
    {
        httpClient = new HttpClient();
        
        try
        {
            if (!apiUrl.StartsWith("https://")) apiUrl = "https://" + apiUrl;
        
            this.apiUrl = apiUrl;
            httpClient.BaseAddress = new Uri(apiUrl);
        
            var loginRequest = new
            {
                email, password
            };
        
            var json = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/Auth/Login", content);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                authToken1 = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse)?.Token;
            }

            if (!string.IsNullOrEmpty(authToken1))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken1);
                var challengeResponse = await httpClient.PostAsync("/Auth/Challenge/BackOffice", null);

                if (challengeResponse.IsSuccessStatusCode)
                {
                    authToken2 = await challengeResponse.Content.ReadAsStringAsync();
                }
            }
            
            return authToken1 != null && authToken2 != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> LoginWithSecondaryPassword(string securePassword)
    {
        try
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(apiUrl);
            
            var loginRequest = new
            {
                solution = securePassword
            };
            
            var json = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
        
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken2);
        
            var upgradeResponse = await httpClient.PostAsync("Auth/Upgrade", content);

            return upgradeResponse.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}

public class LoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}