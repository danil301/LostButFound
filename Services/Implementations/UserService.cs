﻿using LostButFound.API.DAL.Interfaces;
using LostButFound.API.Domian;
using LostButFound.API.Domian.Enum;
using LostButFound.API.Domian.Response;
using LostButFound.API.Domian.ViewModels;
using LostButFound.API.Services.Helpers;
using LostButFound.API.Services.Interfaces;
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

        public static UserViewModel uvm;

        public static string Code;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }



        private async Task<BaseResponse<bool>> CheckExist(string email, string login)
        {
            var users = await _userRepository.Select();
            var Email = users.Where(x => x.Email == email);
            var Login = users.Where(x => x.Login == login);

            if (Email == null)
            {
                return new BaseResponse<bool>()
                {
                    Data = false,
                    Description = "Пользователь с такой почтой уже есть",
                    StatusCode = StatusCode.AlreadyExist
                };
            }

            if (Login == null)
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
                emailSender.SendEmailCode(userViewModel.Email, Code);
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

        public async Task<BaseResponse<ClaimsIdentity>> Register(string code)
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
                    var result = Authenticate(user);

                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Data = result,
                        Description = "Пользователь добавился",
                        StatusCode = StatusCode.Created
                    };
                }
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = "Неверный код",
                    StatusCode = StatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.IternalServerError
                };
            }
        }



        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginVeiwModel model)
        {
            try
            {
                var user = await _userRepository.GetByName(model.Login);
                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден",
                    };
                }

                string pas = String.Empty;
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password.Trim()));
                    pas = BitConverter.ToString(hashValue).Replace("-", "");
                }

                if (user.Password != pas)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пароль не верный",
                    };
                }
                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.IternalServerError
                };
            }
        }


        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
            };
            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
