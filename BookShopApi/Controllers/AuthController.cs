using BookShopApi.Models;
using BookShopApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BookShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }
       
        [HttpPost]
        public IActionResult Token(Auth userData)
        {
            var identity = GetIdentity(userData.email, userData.password, false);
            var identityRefresh = GetIdentity(userData.email, userData.password, true);

            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var jwtRefresh = new JwtSecurityToken(
                   issuer: AuthOptions.ISSUER,
                   audience: AuthOptions.AUDIENCE,
                   notBefore: now,
                   claims: identityRefresh.Claims,
                   expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME_REFRESH)),
                   signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var encodedJwtRefresh = new JwtSecurityTokenHandler().WriteToken(jwtRefresh);

            var response = new
            {
                token = encodedJwt,                    
                refreshToken = encodedJwtRefresh
            };

            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string userEmail, string password, bool isRefresh)
        {
            User person = _userService.GetByEmail(userEmail);
            string role = _userService.GetRole(userEmail);
            //var verifyPassword = VerifyHashedPassword(person.password, password);

            if(!VerifyHashedPassword(person.password, password))
            {
                return null;
            }

            if (person != null)
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

            // если пользователя не найдено
            return null;
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
