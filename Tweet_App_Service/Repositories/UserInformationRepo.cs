using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_Service.DataContext;
using Tweet_App_Service.Models;

namespace Tweet_App_Service.Repositories
{
    public class UserInformationRepo : IUserInformationRepo
    {
        static readonly log4net.ILog _logging = log4net.LogManager.GetLogger(typeof(UserInformationRepo));
        private IMongoCollection<UserInfo> _userInfoCollection;        
        public UserInformationRepo(IDBContext dBContext)
        {            
            _userInfoCollection = dBContext.GetUserInfoCollection();   
        }

        public async Task<BaseResponse<UserInfoResponse>> UserRegistration(UserInfo userInfo)
        {
            BaseResponse<UserInfoResponse> response = new BaseResponse<UserInfoResponse>();
            response.Result = new UserInfoResponse();
            try
            {
                var validationResult = CheckIsUsernameValid(userInfo.LoginId, userInfo.Email);
                if(!validationResult["name"] || !validationResult["email"])
                {
                    response.IsSuccess = false;
                    response.ErrorInfo = !validationResult["name"] ? "Username already taken" : "Email is already registered with us";
                    response.HttpStatusCode = StatusCodes.Status403Forbidden;
                    return response;
                }
                await _userInfoCollection.InsertOneAsync(userInfo);
                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status201Created;
                response.Result.ResponseMessage = $"Dear {userInfo.LoginId} Your Account Created Successfully";
            }
            catch (Exception ex)
            {
                _logging.Error($"Exception Occured While User Registeration Exception Message : {ex.Message} Stack Trace : {ex.StackTrace}");
                response.IsSuccess = false;
                response.ErrorInfo = "Internal Server Error.Please Try Again Later";
                response.HttpStatusCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }

        public async Task<BaseResponse<List<UserInfo>>> GetAllUsers()
        {
            BaseResponse<List<UserInfo>> response = new BaseResponse<List<UserInfo>>();
            response.Result = new List<UserInfo>();
            try
            {
                response.Result = await _userInfoCollection.FindAsync(user => true).Result.ToListAsync();
                
                if(response.Result.Count == 0)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    response.ErrorInfo = "No Users Found";
                    return response;
                }

                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _logging.Error($"Exception Occured While User Registeration Exception Message : {ex.Message} Stack Trace : {ex.StackTrace}");
                response.IsSuccess = false;
                response.ErrorInfo = "Internal Server Error.Please Try Again Later";
                response.HttpStatusCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }

        public async Task<BaseResponse<List<UserInfo>>> SearchUsersbyUsername(string username)
        {
            BaseResponse<List<UserInfo>> response = new BaseResponse<List<UserInfo>>();
            response.Result = new List<UserInfo>();
            try
            {
                //TODO change the filter condition to 'like'
                //response.Result.UserInfo = await _userInfoCollection.FindAsync(user => user.LoginId == username).Matches();

                var filter = Builders<UserInfo>.Filter.Regex("LoginId", new BsonRegularExpression(username));
                response.Result = await _userInfoCollection.Find(filter).ToListAsync();
                if (response.Result.Count == 0)
                {
                    response.IsSuccess = false;
                    response.ErrorInfo = $"No User Found For Given Value : {username}";
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    return response;
                }
                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _logging.Error($"Exception Occured While User Registeration Exception Message : {ex.Message} Stack Trace : {ex.StackTrace}");
                response.IsSuccess = false;
                response.ErrorInfo = "Internal Server Error.Please Try Again Later";
                response.HttpStatusCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }

        private Dictionary<string,bool> CheckIsUsernameValid(string name,string email)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            result.Add("name", false);
            result.Add("email", false);
            UserInfo usernames = _userInfoCollection.Find(users => users.LoginId == name).FirstOrDefault();
            UserInfo emails = _userInfoCollection.Find(users => users.Email == email).FirstOrDefault();
            if(usernames== null && emails == null)
            {
                result["name"] = true;
                result["email"] = true;
                return result;
            }
            else
            {
                if(usernames != null && emails == null)
                {
                    result["name"] = false;
                    result["email"] = true;
                    return result;
                }
                if(emails != null && usernames == null)
                {
                    result["name"] = true;
                    result["email"] = false;
                    return result;
                }
            }

            return result;
        }

    }
}
