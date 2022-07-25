using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Tweet_App_Service.DataContext;
using Tweet_App_Service.Models;

namespace Tweet_App_Service.Repositories
{
    public class AuthenticationRepo : IAuthenticationRepo
    {
        static readonly log4net.ILog _logging = log4net.LogManager.GetLogger(typeof(AuthenticationRepo));
        private IMongoCollection<UserInfo> _userInfoCollection;
        private IConfiguration _config;
        public AuthenticationRepo(IDBContext dBContext, IConfiguration config)
        {
            _userInfoCollection = dBContext.GetUserInfoCollection();
            _config = config;
        }

        public async Task<BaseResponse<LoginResponse>> UserLogin(LoginDTO loginInfo)
        {
            BaseResponse<LoginResponse> response = new BaseResponse<LoginResponse>();
            response.Result = new LoginResponse();
            UserInfo user = new UserInfo();
            try
            {
                user = await (_userInfoCollection.FindAsync(user => user.LoginId == loginInfo.LoginId).Result.FirstOrDefaultAsync());
                if(user == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    response.ErrorInfo = $"User Account Details Not Found for this LoginId : {loginInfo.LoginId}";
                    return response;
                }

                if(!String.Equals(user.Password,loginInfo.Password))
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status403Forbidden;
                    response.ErrorInfo = $"Incorrect Password for LoginId : {loginInfo.LoginId}";
                    return response;
                }

                var loginResponse = new LoginResponse();
                loginResponse.BearerToken = GenerateJsonWebToken();  //TODO : Need to generate the JWT Bearer Token with Login ID in claims
                loginResponse.LoginMessage = "Login Successfull";
                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
                response.Result = loginResponse;
            }
            catch (Exception ex)
            {
                _logging.Error($"Exception Occured While Password Reset Exception Message : {ex.Message} Stack Trace : {ex.StackTrace}");
                response.IsSuccess = false;
                response.HttpStatusCode = StatusCodes.Status500InternalServerError;
                response.ErrorInfo = $"Something Went Wrong,Please Try Again Later";
            }

            return response;
        }

        public async Task<BaseResponse<LoginResponse>> ForgotPassword(string username,string newpassword)
        {
            BaseResponse<LoginResponse> response = new BaseResponse<LoginResponse>();
            response.Result = new LoginResponse();
            UserInfo user = new UserInfo();
            try
            {
                user = await (_userInfoCollection.FindAsync(user => user.LoginId == username).Result.FirstOrDefaultAsync());
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    response.ErrorInfo = $"User Account Details Not Found for this LoginId : {username}";
                    return response;
                }
                var updatedResult = _userInfoCollection.UpdateOne(
                     user => user.LoginId == username,
                     Builders<UserInfo>.Update.Set(userinfo => userinfo.Password, newpassword)
                  );
                bool result = (updatedResult.IsAcknowledged && updatedResult.ModifiedCount > 0);

                if (!result)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status500InternalServerError;
                    response.ErrorInfo = "Updation Failed,Something Went Wrong Try Again Later";
                    return response;
                }
                var loginResponse = new LoginResponse();                
                loginResponse.LoginMessage = "Password Reseted Successfully";
                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
                response.Result = loginResponse;
            }
            catch (Exception ex)
            {
                _logging.Error($"Exception Occured While Password Reset Exception Message : {ex.Message} Stack Trace : {ex.StackTrace}");
                response.IsSuccess = false;
                response.HttpStatusCode = StatusCodes.Status500InternalServerError;
                response.ErrorInfo = $"Something Went Wrong,Please Try Again Later";
            }

            return response;
        }

        public string GenerateJsonWebToken()
        {
            string tokenKey = _config.GetValue<string>("JsonWebTokenKey");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescripter = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = credentials                
            };

            var tokenHandeler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandeler.CreateToken(tokenDescripter);
            var jwtToken = tokenHandeler.WriteToken(securityToken);

            return jwtToken;
        }
    }
}
