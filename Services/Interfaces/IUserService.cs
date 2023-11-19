using LostButFound.API.Domian;
using LostButFound.API.Domian.Response;
using LostButFound.API.Domian.ViewModels;
using System.Security.Claims;

namespace LostButFound.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByLogin(string login);
        Task<BaseResponse<string>> Register(string code);

        BaseResponse<string> SendCode(UserViewModel userViewModel);

        BaseResponse<string> ResendCode();

        Task<BaseResponse<string>> Login(LoginVeiwModel loginVeiwModel);
    }
}
