namespace TokenApp.Services;

public class TokenService
{
    public string GenerateToken(string alias)
    {
        return CreateMD5(alias + DateTime.Now.ToString("yyyyMMddHHmm")).Substring(0,6).ToUpper();
    }
    
    private string CreateMD5(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes); // .NET 5 +
        }
    }
}