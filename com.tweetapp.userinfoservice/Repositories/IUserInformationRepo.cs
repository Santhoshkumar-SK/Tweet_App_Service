using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.userinfoservice.Models;

namespace com.tweetapp.userinfoservice.Repositories
{
    public interface IUserInformationRepo
    {
        public Task<BaseResponse<UserInfoResponse>> UserRegistration(UserInfo userInfo);
        public Task<BaseResponse<List<UserInfo>>> GetAllUsers();
        public Task<BaseResponse<List<UserInfo>>> SearchUsersbyUsername(string username);
    }
}
