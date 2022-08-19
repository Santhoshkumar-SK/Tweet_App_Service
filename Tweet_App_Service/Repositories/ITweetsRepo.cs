using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_Service.Models;

namespace Tweet_App_Service.Repositories
{
    public interface ITweetsRepo
    {
        public Task<BaseResponse<TweetsResponse>> PostTweet(PostedTweet tweets);
        public Task<BaseResponse<TweetsResponse>> ReplyTweet(PostedTweet tweets, string orgintweetid);
        public Task<BaseResponse<TweetsResponse>> UpdateTweet(PostedTweet tweet, string tweetId);
        public Task<BaseResponse<TweetsResponse>> LikeTweet(string tweetId,string username);
        public Task<BaseResponse<TweetsResponse>> DeleteTweet(string tweetId);
        public Task<BaseResponse<List<PostedTweet>>> GetallTweets();
        public Task<BaseResponse<List<PostedTweet>>> GetallTweetsForUser(string username);
        public Task<BaseResponse<List<PostedTweet>>> GetallReplysForTweet(string tweetId);
    }
}
