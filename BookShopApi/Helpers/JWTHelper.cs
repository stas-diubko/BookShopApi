using BookShopApi.Models;
using BookShopApi.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BookShopApi.Helpers
{
    public class JWTHelper
    {
        private readonly UserService _userService;

        public JWTHelper(UserService userService)
        {
            _userService = userService;
        }

        public JWTAuthResponse GenerateToken(Auth userData)
        {

            User person = _userService.GetByEmail(userData.email);
            string role = _userService.GetRole(userData.email);

            if (person == null)
            {
                return null;
            }

            if (!VerifyHashedPassword(person.password, userData.password))
            {
                return null;
            }

            var identity = GetIdentity(person, role, false);
            var identityRefresh = GetIdentity(person, role, true);

            var encodedJwt = GetJWT(identity, false);
            var encodedJwtRefresh = GetJWT(identityRefresh, true);

            return new JWTAuthResponse
            {
                token = encodedJwt,
                refreshToken = encodedJwtRefresh
            };
        }

        public JWTAuthResponse refreshToken(User user)
        {
            var role = _userService.GetRole(user.email);
            var identity = GetIdentity(user, role, false);
            var identityRefresh = GetIdentity(user, role, true);
            var encodedJwt = GetJWT(identity, false);
            var encodedJwtRefresh = GetJWT(identityRefresh, true);

            return new JWTAuthResponse
            {
                token = encodedJwt,
                refreshToken = encodedJwtRefresh
            };
        }

        public string GetJWT(ClaimsIdentity identity, bool isRefresh)
        {
            var now = DateTime.UtcNow;
            if (isRefresh)
            {
                var jwt = new JwtSecurityToken(
                                   issuer: AuthOptions.ISSUER,
                                   audience: AuthOptions.AUDIENCE,
                                   notBefore: now,
                                   claims: identity.Claims,
                                   expires: now.Add(TimeSpan.FromSeconds(AuthOptions.LIFETIME_REFRESH)),
                                   signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                return encodedJwt;
            } else
            {
                var jwt = new JwtSecurityToken(
                                    issuer: AuthOptions.ISSUER,
                                    audience: AuthOptions.AUDIENCE,
                                    notBefore: now,
                                    claims: identity.Claims,
                                    expires: now.Add(TimeSpan.FromSeconds(AuthOptions.LIFETIME)),
                                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                return encodedJwt;
            }
        }

        private ClaimsIdentity GetIdentity(User person, string role, bool isRefresh)
        { 
                var claims = isRefresh ?
                                new List<Claim>
                                {
                                    new Claim("id", person._id),
                                }
                                :
                                new List<Claim>
                                {
                                    new Claim("id", person._id),
                                    new Claim("name", person.name),
                                    new Claim("email", person.email),
                                    new Claim("role", role)
                                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return buffer3.SequenceEqual(buffer4);

        }
    }
}
