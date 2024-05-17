using LostButFound.API.DAL.Interfaces;
using LostButFound.API.Domian;
using LostButFound.API.Domian.Enum;
using LostButFound.API.Domian.Response;
using LostButFound.API.Domian.ViewModels;
using LostButFound.API.Services.Helpers;
using LostButFound.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace LostButFound.API.Services.Implementations
{
    public class UserService : IUserService
    {
        public IUserRepository _userRepository;
        public IThingRepository _thingRepository;

        private readonly IConfiguration _configuration;

        public static UserViewModel uvm;

        public static string Code;

        public UserService(IUserRepository userRepository, IThingRepository thingRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _thingRepository = thingRepository;
            _configuration = configuration;
        }

        public async Task<User> GetUserByLogin(string login)
        {
            return await _userRepository.GetByName(login);
        }

        public async Task<BaseResponse<string>> UpdateUserLogin(string login, string newLogin)
        {
            User user = await _userRepository.GetByName(login);
            var things = _thingRepository.Select().Result.Where(x => x.UserName == login);
            user.Login = newLogin;
            var response = await _userRepository.Update(user);
         
            if (response)
            {
                foreach (var item in things)
                {
                    item.UserName = newLogin;
                    await _thingRepository.Update(item);
                }
                
                var token = GenerateAuthorizationToken(user);
                return new BaseResponse<string>
                {
                    Data = token,
                    StatusCode = StatusCode.OK,
                    Description = "Логин изменился"
                };
            }
            return new BaseResponse<string>
            {
                StatusCode = StatusCode.IternalServerError,
                Description = "Ошибка при изменении логина"
            };
        }

        private async Task<BaseResponse<bool>> CheckExist(string email, string login)
        {
            var users = await _userRepository.Select();
            var Email = users.FirstOrDefault(x => x.Email == email);
            var Login = users.FirstOrDefault(x => x.Login == login);

            if (Email != null)
            {
                return new BaseResponse<bool>()
                {
                    Data = false,
                    Description = "Пользователь с такой почтой уже есть",
                    StatusCode = StatusCode.AlreadyExist
                };
            }

            if (Login != null)
            {
                return new BaseResponse<bool>()
                {
                    Data = false,
                    Description = "Пользователь с таким логином уже есть",
                    StatusCode = StatusCode.AlreadyExist
                };
            }

            return new BaseResponse<bool>()
            {
                Data = true
            };
        }

        public BaseResponse<string> SendCode(UserViewModel userViewModel)
        {
            if(CheckExist(userViewModel.Email, userViewModel.Login).Result.Data)
            {
                Cryptographer cryptographer = new Cryptographer();
                Code = cryptographer.GenerateEmailCode();
                uvm = userViewModel;
                EmailSender emailSender = new EmailSender();
                emailSender.SendEmailCode(userViewModel.Email, Code, true);
                return new BaseResponse<string>()
                {
                    Description = Code,
                    Data = "Пользователю выслан код",
                    StatusCode = StatusCode.OK
                };
            }
            return new BaseResponse<string>()
            {
                Description = CheckExist(userViewModel.Email, userViewModel.Login).Result.Description,
                StatusCode = StatusCode.AlreadyExist
            };

        }

        public async Task<BaseResponse<string>> Register(string code)
        {
            try
            {
                if(code == Code)
                {
                    Cryptographer cryptographer = new Cryptographer();
                    User user = new User()
                    {
                        FullName = uvm.FullName,
                        Login = uvm.Login,
                        Password = cryptographer.CryptPassword(uvm.Password),
                        Email = uvm.Email,
                        Role = "User",
                        Rating = 0,
                    };

                    await _userRepository.Create(user);

                    return new BaseResponse<string>()
                    {
                        Description = "Пользователь добавился",
                        StatusCode = StatusCode.Created
                    };
                }
                return new BaseResponse<string>()
                {
                    Description = "Неверный код",
                    StatusCode = StatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new BaseResponse<string>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.IternalServerError
                };
            }
        }

        public BaseResponse<string> ResendCode()
        {
            Cryptographer cryptographer = new Cryptographer();
            Code = cryptographer.GenerateEmailCode();
            EmailSender emailSender = new EmailSender();
            emailSender.SendEmailCode(uvm.Email, Code, true);
            return new BaseResponse<string>()
            {
                Description = Code,
                Data = "Пользователю повторно выслан код",
                StatusCode = StatusCode.OK
            };
        }

        public async Task<BaseResponse<string>> Login(LoginVeiwModel model)
        {
            try
            {                
                var user = _userRepository.Select().Result.FirstOrDefault(x => x.Login == model.LoginOrEmail || x.Email == model.LoginOrEmail);              
                
                if (user == null)
                {
                    return new BaseResponse<string>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }
                Cryptographer cryptographer = new Cryptographer();
                string pas = cryptographer.DecryptPassword(model.Password);

                if (user.Password != pas)
                {
                    return new BaseResponse<string>()
                    {
                        StatusCode = StatusCode.Unauthorized,
                        Description = "Пароль не верный",
                    };
                }
                var resp = GenerateAuthorizationToken(user);
                return new BaseResponse<string>()
                {
                    Data = resp,
                    Description = "Пользователь авторизован",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.IternalServerError
                };
            }
        }

        public async Task<BaseResponse<string>> SendEmailForChangePassword(string email)
        {
            var user = _userRepository.Select().Result.FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                EmailSender emailSender = new EmailSender();
                Cryptographer cryptographer = new Cryptographer();

                emailSender.SendEmailCode(email, $"http://localhost:5173/updateuserpassword?id={user.Id.ToString()}", false);
                return new BaseResponse<string>()
                {
                    Description = Code,
                    Data = "Пользователю выслана ссылка",
                    StatusCode = StatusCode.OK
                };
            }
            return new BaseResponse<string>() 
            {
                Data = "Пользователь не найден",
                StatusCode = StatusCode.NotFound
            };
        }



        public async Task<BaseResponse<string>> ConfirmChangePassword(string id, string newPassword)
        {
            Cryptographer cryptographer = new Cryptographer();
            //id = cryptographer.DecryptPassword(id);
            var user = _userRepository.Get(int.Parse(id)).Result;

            if (user != null)
            {
                user.Password = cryptographer.CryptPassword(newPassword);
                await _userRepository.Update(user);
                return new BaseResponse<string>()
                {
                    StatusCode = StatusCode.OK
                };
            }

            return new BaseResponse<string>()
            {
                StatusCode = StatusCode.IternalServerError
            };
        }

        private string GenerateAuthorizationToken(User user)
        {
            var secret = "secret*secret123secret444";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            var userClaims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
            };

            var jwt = new JwtSecurityToken(
                    claims: userClaims,
                    expires: DateTime.Now.AddHours(2),
                    audience: "https://localhost:7000/",
                    issuer: "https://localhost:7000/",
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

    }
}
