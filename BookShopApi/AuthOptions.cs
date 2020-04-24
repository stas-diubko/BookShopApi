using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookShopApi
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        const string KEY = "mysupersecret_secretkey!123";
        public const int LIFETIME = 30; // token's lifetime - 1 min
        public const int LIFETIME_REFRESH = 60; // token's lifetime - 1 min
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
