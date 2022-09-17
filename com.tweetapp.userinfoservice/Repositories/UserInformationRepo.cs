using com.tweetapp.userinfoservice.DataContext;
using com.tweetapp.userinfoservice.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.tweetapp.userinfoservice.Repositories
{
    public class UserInformationRepo : IUserInformationRepo
    {
        static readonly log4net.ILog _logging = log4net.LogManager.GetLogger(typeof(UserInformationRepo));
        private IMongoCollection<UserInfo> _userInfoCollection;
        private ProducerConfig _producerConfig;
        private IConfiguration _config;
        public UserInformationRepo(IDBContext dBContext, ProducerConfig producerConfig,IConfiguration config)
        {
            _userInfoCollection = dBContext.GetUserInfoCollection();
            _config = config;
            _producerConfig = producerConfig;
        }

        public async Task<BaseResponse<UserInfoResponse>> UserRegistration(UserInfo userInfo)
        {
            BaseResponse<UserInfoResponse> response = new BaseResponse<UserInfoResponse>();
            response.Result = new UserInfoResponse();
            try
            {
                
                var validationResult = CheckIsUsernameValid(userInfo.LoginId, userInfo.Email);
                if (!validationResult["name"] || !validationResult["email"])
                {
                    response.IsSuccess = false;
                    response.ErrorInfo = !validationResult["name"] ? "Username already taken" : "Email is already registered with us";
                    response.HttpStatusCode = StatusCodes.Status403Forbidden;
                    return response;
                }
                await _userInfoCollection.InsertOneAsync(userInfo);
                KafkaProducerData dataTosend = new KafkaProducerData()
                {
                    UserInterests = userInfo.Interests,
                    UserName = userInfo.LoginId
                };
                bool resultFromKafka = SendDataToTweetServiceAsync(dataTosend).GetAwaiter().GetResult();
                
                if (resultFromKafka)
                {
                    response.IsSuccess = true;
                    response.HttpStatusCode = StatusCodes.Status201Created;
                    response.Result.ResponseMessage = $"Dear {userInfo.LoginId} Your Account Created Successfully";
                    return response;
                }
                response.IsSuccess = false;
                response.HttpStatusCode = StatusCodes.Status201Created;
                response.Result.ResponseMessage = $"Dear {userInfo.LoginId} Your Account Created but Interestes not created";
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

        private async Task<bool> SendDataToTweetServiceAsync(KafkaProducerData data)
        {            
            string jsonData = JsonSerializer.Serialize(data);
            string topicname = _config.GetSection("KafkaSettings").GetValue<string>("TopicName");
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                await producer.ProduceAsync(topicname, new Message<Null, string> { Value = jsonData });
                //producer.Flush(TimeSpan.FromSeconds(10));
                return true;
            }
        }

        public async Task<BaseResponse<List<UserInfo>>> GetAllUsers()
        {
            BaseResponse<List<UserInfo>> response = new BaseResponse<List<UserInfo>>();
            response.Result = new List<UserInfo>();
            try
            {
                response.Result = await _userInfoCollection.FindAsync(user => true).Result.ToListAsync();

                if (response.Result.Count == 0)
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

        private Dictionary<string, bool> CheckIsUsernameValid(string name, string email)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            result.Add("name", false);
            result.Add("email", false);
            UserInfo usernames = _userInfoCollection.Find(users => users.LoginId == name).FirstOrDefault();
            UserInfo emails = _userInfoCollection.Find(users => users.Email == email).FirstOrDefault();
            if (usernames == null && emails == null)
            {
                result["name"] = true;
                result["email"] = true;
                return result;
            }
            else
            {
                if (usernames != null && emails == null)
                {
                    result["name"] = false;
                    result["email"] = true;
                    return result;
                }
                if (emails != null && usernames == null)
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
