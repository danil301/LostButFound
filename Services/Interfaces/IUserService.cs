using LostButFound.API.Domian;
using LostButFound.API.Domian.Response;
using LostButFound.API.Domian.ViewModels;
using System.Security.Claims;

namespace LostButFound.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByLogin(string login);
        Task<BaseResponse<ClaimsIdentity>> Register(string code);

        BaseResponse<string> SendCode(UserViewModel userViewModel);

        BaseResponse<string> ResendCode();

        Task<BaseResponse<ClaimsIdentity>> Login(LoginVeiwModel loginVeiwModel);
    }
}
