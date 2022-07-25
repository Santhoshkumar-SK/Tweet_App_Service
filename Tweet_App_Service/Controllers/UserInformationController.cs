using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_Service.Models;
using Tweet_App_Service.Repositories;

namespace Tweet_App_Service.Controllers
{
    [Route("api/v1.0/tweets")]
    [ApiController]
    public class UserInformationController : ControllerBase
    {
        IUserInformationRepo _userInfoRepo;
        public UserInformationController(IUserInformationRepo userInfoRepo)
        {
            _userInfoRepo = userInfoRepo;
        }

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<UserInfoResponse>>> UserRegisteration(UserInfo userInfo)
        {
            BaseResponse<UserInfoResponse> response = await _userInfoRepo.UserRegistration(userInfo);
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("users/all")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<UserInfo>>>> GetAllUsers()
        {
            BaseResponse<List<UserInfo>> response = await _userInfoRepo.GetAllUsers();
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("users/search/{username}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<UserInfo>>>> SearchUsersbyUsername(string username)
        {
            BaseResponse<List<UserInfo>> response = await _userInfoRepo.SearchUsersbyUsername(username);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
