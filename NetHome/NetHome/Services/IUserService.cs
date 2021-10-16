﻿using NetHome.Common.Models;
using System.Threading.Tasks;

namespace NetHome.Services
{
    public interface IUserService
    {
        Task<bool> Login(LoginModel loginModel);
        Task<bool> Validate();
        Task<bool> Register(RegisterModel registerModel);
        UserModel GetUserData();
        void ClearUserData();
    }
}
