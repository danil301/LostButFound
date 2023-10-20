using LostButFound.API.Domian.Response;
using LostButFound.API.Domian.ViewModels;
using System.Security.Claims;

namespace LostButFound.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<ClaimsIdentity>> Register(string code);

        BaseResponse<string> SendCode(UserViewModel userViewModel);

        //ClaimsIdentity Login(LoginViewModel model);
    }
}
