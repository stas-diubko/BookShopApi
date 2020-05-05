using BookShopApi.Helpers;
using BookShopApi.Models;
using BookShopApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private JWTHelper _jwtHelper;
        private readonly UserService _userService;

        public AuthController(JWTHelper jwtHelper, UserService userService)
        {
            _jwtHelper = jwtHelper;
            _userService = userService;
        }
       
        [HttpPost]
        public IActionResult Authenticate(Auth userData)
        {
            var response = _jwtHelper.GenerateToken(userData);
            
            if (response == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            return Json(response);
        }

        [HttpPost("{refreshToken}")]
        public async Task<IActionResult> RefreshToken(RequestRefreshToken refreshData)
        {
            var decodedToken = new JwtSecurityTokenHandler().ReadToken(refreshData.refreshToken) as JwtSecurityToken;
            var userId = decodedToken.Claims.First(claim => claim.Type == "id").Value;
            var expirationTime = Int32.Parse(decodedToken.Claims.First(claim => claim.Type == "exp").Value);
            var now = DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds;
            if(expirationTime < now)
            {
                return BadRequest(new { errorText = "Token expired" });
            }
            User user = _userService.Get(userId);

            if (user == null)
            {
                return NotFound();
            }

            JWTAuthResponse response = _jwtHelper.refreshToken(user);

            return Json(response);
        }
    }
}
