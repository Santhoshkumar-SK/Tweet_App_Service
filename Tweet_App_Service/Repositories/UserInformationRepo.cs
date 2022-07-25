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

    }
}
