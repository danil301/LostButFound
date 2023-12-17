using LostButFound.API.DAL.Interfaces;
using LostButFound.API.DAL.Repositories;
using LostButFound.API.Domian;
using LostButFound.API.Domian.ViewModels;
using LostButFound.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LostButFound.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/{id?}")]
    public class UserController : Controller
    {
        public IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel userViewModel)
        {
            if(ModelState.IsValid)
            {
                var response = _userService.SendCode(userViewModel);
                if (response.StatusCode == Domian.Enum.StatusCode.AlreadyExist)
                {
                    return BadRequest(new { message = response.Description });
                }
                if (response.StatusCode == Domian.Enum.StatusCode.OK)
                {
                    return Ok(response.Description);
                }
                ModelState.AddModelError("", response.Description);
            }
            return Ok("User has been registered");
        }

        [HttpPost]
        public IActionResult ConfirmRegister(string code)
        {
            var response = _userService.Register(code);
            if (response.Result.StatusCode == Domian.Enum.StatusCode.Created)
            {
                return Ok(response.Result.Description);
            }
            return BadRequest(response.Result.Description);
        }

        [HttpGet]
        public IActionResult ResendCode()
        {
            var response = _userService.ResendCode();
            return Ok(response.Description);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVeiwModel loginVeiwModel)
        {
            var response = await _userService.Login(loginVeiwModel);

            if (response.StatusCode == Domian.Enum.StatusCode.OK)
            {
                return Ok(response.Data);
            }
            if (response.StatusCode == Domian.Enum.StatusCode.NotFound) return BadRequest(response.Description);
            if (response.StatusCode == Domian.Enum.StatusCode.Unauthorized) return BadRequest(response.Description);
            return BadRequest(response.Description);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<User> GetCurrentUser()
        {
            string login = User.Identity.Name;
            return await _userService.GetUserByLogin(login);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateLogin(string newLogin)
        {
            string login = User.Identity.Name;
            var response = await _userService.UpdateUserLogin(login, newLogin);
            if (response.StatusCode == Domian.Enum.StatusCode.OK)
            {
                login = User.Identity.Name;
                var tok = response.Data;
                return Ok(response.Data);
            }

            return BadRequest();
        }
    }

   
}
