using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.userinfoservice.Models;

namespace com.tweetapp.userinfoservice.Repositories
{
    public interface IAuthenticationRepo
    {
        public Task<BaseResponse<LoginResponse>> UserLogin(LoginDTO loginInfo);
        public Task<BaseResponse<LoginResponse>> ForgotPassword(string username, string newpassword);
    }
}
