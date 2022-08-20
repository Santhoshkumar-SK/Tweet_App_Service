using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.tweetinfoservice.Models;
using com.tweetapp.tweetinfoservice.Repositories;

namespace com.tweetapp.tweetinfoservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TweetsController : ControllerBase
    {
        ITweetsRepo _tweetsRepo;

        public TweetsController(ITweetsRepo tweetsRepo)
        {
            _tweetsRepo = tweetsRepo;
        }

        [Route("{username}/add")]
        [HttpPost]        
        public async Task<ActionResult<BaseResponse<TweetsResponse>>> PostTweet(PostedTweet tweets)
        {
            BaseResponse<TweetsResponse> response = await _tweetsRepo.PostTweet(tweets);
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("{username}/reply/{id}")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<TweetsResponse>>> ReplyTweet(PostedTweet tweets,string id)
        {
            BaseResponse<TweetsResponse> response = await _tweetsRepo.ReplyTweet(tweets,id);
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("{username}/update/{id}")]
        [HttpPut]
        public async Task<ActionResult<BaseResponse<TweetsResponse>>> UpdateTweet(PostedTweet tweet, string id)
        {
            BaseResponse<TweetsResponse> response = await _tweetsRepo.UpdateTweet(tweet, id);
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("{username}/like/{id}")]
        [HttpPut]
        public async Task<ActionResult<BaseResponse<TweetsResponse>>> LikeTweet(string username,string id)
        {
            BaseResponse<TweetsResponse> response = await _tweetsRepo.LikeTweet(id,username);
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("{username}/delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult<BaseResponse<TweetsResponse>>> DeleteTweet(string id)
        {
            BaseResponse<TweetsResponse> response = await _tweetsRepo.DeleteTweet(id);
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("all")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<PostedTweet>>>> GetAllTweets()
        {
            BaseResponse<List<PostedTweet>> response = await _tweetsRepo.GetallTweets();
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("{username}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<PostedTweet>>>> GetAllTweetsForUser(string username)
        {
            BaseResponse<List<PostedTweet>> response = await _tweetsRepo.GetallTweetsForUser(username);
            return StatusCode(response.HttpStatusCode, response);
        }

        [Route("reply/{id}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<PostedTweet>>>> GetallReplysForTweet(string id)
        {
            BaseResponse<List<PostedTweet>> response = await _tweetsRepo.GetallReplysForTweet(id);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
