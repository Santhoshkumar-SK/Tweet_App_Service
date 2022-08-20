using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.tweetinfoservice.DataContext;
using com.tweetapp.tweetinfoservice.Models;

namespace com.tweetapp.tweetinfoservice.Repositories
{
    public class TweetsRepo : ITweetsRepo
    {
        static readonly log4net.ILog _logging = log4net.LogManager.GetLogger(typeof(TweetsRepo));
        private IMongoCollection<PostedTweet> _tweetsCollection;

        public TweetsRepo(IDBContext dBContext)
        {
            _tweetsCollection = dBContext.GetTweetsCollection();
        }

        public async Task<BaseResponse<TweetsResponse>> PostTweet(PostedTweet tweets)
        {
            BaseResponse<TweetsResponse> response = new BaseResponse<TweetsResponse>();
            response.Result = new TweetsResponse();
            try
            {
                tweets.TweetId = Guid.NewGuid().ToString();
                tweets.TimeofPost =  DateTime.Now ;
                await _tweetsCollection.InsertOneAsync(tweets);
                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status201Created;
                response.Result.ResponseMessage = "Tweet Posted Successfully";
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

        public async Task<BaseResponse<TweetsResponse>> ReplyTweet(PostedTweet tweets, string orgintweetid)
        {
            BaseResponse<TweetsResponse> response = new BaseResponse<TweetsResponse>();
            response.Result = new TweetsResponse();
            try
            {
                //To find the orgin tweet for updating the replies
                var filter = Builders<PostedTweet>.Filter.Where(tw => tw.TweetId == orgintweetid);
                PostedTweet postedTweet = _tweetsCollection.Find(filter).FirstOrDefaultAsync().Result;
                if (postedTweet == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorInfo = "Invaild Tweet ID";
                    return response;
                }

                //To insert a new tweet
                tweets.TweetId = Guid.NewGuid().ToString();
                await _tweetsCollection.InsertOneAsync(tweets);

                //To update the replied tweet id against the orgin tweet.
                postedTweet.RepliedTweetIds.Add(tweets.TweetId);
                var update = Builders<PostedTweet>.Update.Set("RepliedTweetIds", postedTweet.RepliedTweetIds);
                PostedTweet orgintweet = _tweetsCollection.FindOneAndUpdate(filter, update);

                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status201Created;
                response.Result.ResponseMessage = "Tweet Replied Successfully";
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

        public async Task<BaseResponse<TweetsResponse>> UpdateTweet(PostedTweet tweet, string tweetId)
        {
            BaseResponse<TweetsResponse> response = new BaseResponse<TweetsResponse>();
            response.Result = new TweetsResponse();
            try
            {
                //To find and update the tweet.
                var filter = Builders<PostedTweet>.Filter.Where(tw => tw.TweetId == tweetId);
                var update = Builders<PostedTweet>.Update.Set("InsightMessage", tweet.InsightMessage).Set("TweetMessage", tweet.TweetMessage);
                PostedTweet updatedTweet = await _tweetsCollection.FindOneAndUpdateAsync(filter, update);

                if (updatedTweet == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorInfo = "Invalid Tweet ID";
                    return response;
                }

                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
                response.Result.ResponseMessage = "Tweet Updated Successfully";
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

        public async Task<BaseResponse<TweetsResponse>> LikeTweet(string tweetId,string username)
        {
            BaseResponse<TweetsResponse> response = new BaseResponse<TweetsResponse>();
            response.Result = new TweetsResponse();
            try
            {
                var filter = Builders<PostedTweet>.Filter.Where(tw => tw.TweetId == tweetId);
                PostedTweet postedTweet = _tweetsCollection.Find(filter).FirstOrDefaultAsync().Result;
                if (postedTweet == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorInfo = "Invaild Tweet ID";
                    return response;
                }

                //To find and update the tweet.                
                postedTweet.NumberOfLikes += 1;
                postedTweet.LikedUsers.Add(username);
                var update = Builders<PostedTweet>.Update.Set("NumberOfLikes",postedTweet.NumberOfLikes).Set("LikedUsers",postedTweet.LikedUsers);
                PostedTweet updatedTweet = await _tweetsCollection.FindOneAndUpdateAsync(filter, update);

                if (updatedTweet == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorInfo = "Invalid Tweet ID";
                    return response;
                }

                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
                response.Result.ResponseMessage = "Tweet Like Updated Successfully";
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

        public async Task<BaseResponse<TweetsResponse>> DeleteTweet(string tweetId)
        {
            BaseResponse<TweetsResponse> response = new BaseResponse<TweetsResponse>();
            response.Result = new TweetsResponse();
            try
            {
                //To find and delete the tweet.
                var filter = Builders<PostedTweet>.Filter.Where(tw => tw.TweetId == tweetId);
                PostedTweet tweetForDeletion = _tweetsCollection.Find(filter).FirstOrDefaultAsync().Result;

                if (tweetForDeletion == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorInfo = "Invalid Tweet ID";
                    return response;
                }

                if (tweetForDeletion.RepliedTweetIds != null && tweetForDeletion.RepliedTweetIds.Count != 0)
                {
                    var filterforreplies = Builders<PostedTweet>.Filter.Where(tw => tweetForDeletion.RepliedTweetIds.Contains(tw.TweetId));
                    var deletedResult = _tweetsCollection.DeleteMany(filterforreplies);
                }
                PostedTweet deletedTweet = await _tweetsCollection.FindOneAndDeleteAsync(filter);

                if (deletedTweet == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status500InternalServerError;
                    response.ErrorInfo = "Unable To Delete The Tweet";
                    return response;
                }
                response.IsSuccess = true;
                response.HttpStatusCode = StatusCodes.Status200OK;
                response.Result.ResponseMessage = "Tweet Deleted Successfully";
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

        public async Task<BaseResponse<List<PostedTweet>>> GetallTweets()
        {
            BaseResponse<List<PostedTweet>> response = new BaseResponse<List<PostedTweet>>();
            response.Result = new List<PostedTweet>();
            try
            {
                //To get all the tweet.
                response.Result = await _tweetsCollection.FindAsync(tweets => !tweets.IsReplyFlag).Result.ToListAsync();

                if (response.Result == null || response.Result.Count == 0)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    response.ErrorInfo = "No Tweets Found";
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

        public async Task<BaseResponse<List<PostedTweet>>> GetallTweetsForUser(string username)
        {
            BaseResponse<List<PostedTweet>> response = new BaseResponse<List<PostedTweet>>();
            response.Result = new List<PostedTweet>();
            try
            {
                //To get all the tweet.
                response.Result = await _tweetsCollection.FindAsync(tweets => !tweets.IsReplyFlag && tweets.LoginId == username).Result.ToListAsync();

                if (response.Result == null || response.Result.Count == 0)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    response.ErrorInfo = "No Tweets Found";
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

        public async Task<BaseResponse<List<PostedTweet>>> GetallReplysForTweet(string tweetId)
        {
            BaseResponse<List<PostedTweet>> response = new BaseResponse<List<PostedTweet>>();
            response.Result = new List<PostedTweet>();
            try
            {
                //To get all the tweet.
                //To find and delete the tweet.
                var filter = Builders<PostedTweet>.Filter.Where(tw => tw.TweetId == tweetId);
                PostedTweet tweetForGetReply = _tweetsCollection.Find(filter).FirstOrDefaultAsync().Result;

                if (tweetForGetReply == null)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorInfo = "Invalid Tweet ID";
                    return response;
                }

                if (tweetForGetReply.RepliedTweetIds == null || tweetForGetReply.RepliedTweetIds.Count == 0)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    response.ErrorInfo = "No Replies Found";
                    return response;
                }

                var filterforreplies = Builders<PostedTweet>.Filter.Where(tw => tweetForGetReply.RepliedTweetIds.Contains(tw.TweetId));
                response.Result = await _tweetsCollection.FindAsync(filterforreplies).Result.ToListAsync();

                if (response.Result == null || response.Result.Count == 0)
                {
                    response.IsSuccess = false;
                    response.HttpStatusCode = StatusCodes.Status404NotFound;
                    response.ErrorInfo = "No Tweets Found";
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
