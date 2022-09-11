using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SF.BlogApi
{
    public class AuthOptions
    {
        public const string ISSUER = "BlogApiServer";            // издатель токена
        public const string AUDIENCE = "BlogApiClient";          // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";        // ключ для шифрования
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
