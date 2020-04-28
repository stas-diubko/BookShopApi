using BookShopApi.Helpers;
using BookShopApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private JWTHelper _jwtHelper;

        public AuthController(JWTHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
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
    }
}
