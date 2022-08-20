using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.userinfoservice.Models;
using com.tweetapp.userinfoservice.Repositories;

namespace com.tweetapp.userinfoservice.Controllers
{
    [Route("api/[controller]")]
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
        [Authorize]
        public async Task<ActionResult<BaseResponse<List<UserInfo>>>> GetAllUsers()
        {
            BaseResponse<List<UserInfo>> response = await _userInfoRepo.GetAllUsers();
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("users/search/{username}")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<BaseResponse<List<UserInfo>>>> SearchUsersbyUsername(string username)
        {
            BaseResponse<List<UserInfo>> response = await _userInfoRepo.SearchUsersbyUsername(username);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
