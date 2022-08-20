using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_UserInfo_Service.Models;
using Tweet_App_UserInfo_Service.Repositories;

namespace Tweet_App_UserInfo_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationRepo _authRepo;
        public AuthenticationController(IAuthenticationRepo authRepo)
        {
            _authRepo = authRepo;
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> UserLogin(LoginDTO loginInfo)
        {
            BaseResponse<LoginResponse> response = await _authRepo.UserLogin(loginInfo);
            return StatusCode(response.HttpStatusCode, response);
        }


        [HttpGet("{username}/forgot")]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> ForgetPassword(string username,string newpassword)
        {
            BaseResponse<LoginResponse> response = await _authRepo.ForgotPassword(username,newpassword);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
