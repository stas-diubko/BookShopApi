using BookShopApi.Models;
using BookShopApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using BookShopApi.Helpers;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace BookShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private JWTHelper _jwtHelper;

        public UsersController(UserService userService, JWTHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<User>> Get() =>
            _userService.Get();

        [Authorize]
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("{page}/{pageSize}")]
        public ActionResult<ResponseQueryUsers> GetBooksByQuery(int page, int pageSize) =>
         _userService.GetUsersWithQuery(page, pageSize);

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:length(24)}")]
        public ActionResult<ResponseGeneral> Delete(string id)
        {
            var user = _userService.Get(id);

            if(user == null)
            {
                return NotFound();
            }

            _userService.Remove(user._id);

            return new ResponseGeneral() { success = true, message = "User deleted" };
        }

        [HttpPost("{register}")]
        public ActionResult<User> Create(User user)
        {
            var existingUser = _userService.GetByEmail(user.email);
            if (existingUser != null)
            {
                return BadRequest(new { errorText = "Email is already in use" });
            } else
            {
                _userService.Create(user);
                return CreatedAtRoute("GetUser", new { id = user._id.ToString() }, user);
            }
        }

        [Authorize]
        [HttpPut("{id:length(24)}")]
        public ActionResult<ResponseGeneralWithToken> Update(string id, UserUpdating updatingUser)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(id, updatingUser);

            var authData = new Auth();
            authData.password = user.password;
            authData.email = user.email;

            var tokenData = _jwtHelper.GenerateToken(authData);

            return new ResponseGeneralWithToken() { success = true, message = "Data updated", data = tokenData };
        }

        //[Authorize]
        //[HttpGet("{password}/{id:length(24)}")]
        //public void UpdatePassword(string id, PasswordUpdate updatingData)
        //{

        //}

        [Authorize]
        [HttpGet("{avatar}/{id:length(24)}")]
        public async Task<ActionResult<string>> GetUserAvatar(string id)
        {
            string avatar = _userService.GetUserAvatar(id);
            return avatar;
        }
    }
}
