using LostButFound.API.DAL.Interfaces;
using LostButFound.API.DAL.Repositories;
using LostButFound.API.Domian;
using LostButFound.API.Domian.ViewModels;
using LostButFound.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            var x = User.Identity.Name;
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
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data));               
                return Ok(response.Description);
            }
            if (response.StatusCode == Domian.Enum.StatusCode.NotFound) return BadRequest(response.Description);
            if (response.StatusCode == Domian.Enum.StatusCode.Unauthorized) return BadRequest(response.Description);
            return BadRequest(response.Description);
        }

        [HttpGet]
        [Authorize]
        public async Task<User> GetCurrentUser()
        {
            string login = User.Identity.Name;
            return await _userService.GetUserByLogin(login);
        }
    }

   
}
