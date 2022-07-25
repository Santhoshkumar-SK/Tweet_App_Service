using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_Service.Models;

namespace Tweet_App_Service.Repositories
{
    public interface IUserInformationRepo
    {
        public Task<BaseResponse<UserInfoResponse>> UserRegistration(UserInfo userInfo);
        public Task<BaseResponse<List<UserInfo>>> GetAllUsers();
        public Task<BaseResponse<List<UserInfo>>> SearchUsersbyUsername(string username);
    }
}
