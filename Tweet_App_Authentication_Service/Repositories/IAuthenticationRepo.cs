using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_Authentication_Service.Models;

namespace Tweet_App_Authentication_Service.Repositories
{
    public interface IAuthenticationRepo
    {
        public Task<BaseResponse<LoginResponse>> UserLogin(LoginDTO loginInfo);
        public Task<BaseResponse<LoginResponse>> ForgotPassword(string username, string newpassword);
    }
}
