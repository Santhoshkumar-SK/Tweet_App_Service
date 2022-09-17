﻿using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.tweetinfoservice.Models;

namespace com.tweetapp.tweetinfoservice.DataContext
{
    public class DBContext : IDBContext
    {        
        private IMongoDatabase _mongoDatabase;
        private readonly IConfiguration _config;
        private MongoClient _mongoClient;

        public DBContext(IConfiguration config)
        {
            _config = config;

            _mongoClient = new MongoClient(_config.GetSection("TweetAppDDSettings").GetValue<string>("ConnectionString"));

            _mongoDatabase = _mongoClient.GetDatabase(_config.GetSection("TweetAppDDSettings").GetValue<string>("DatabaseName"));                
        }
        
        public IMongoCollection<PostedTweet> GetTweetsCollection() =>
           _mongoDatabase.GetCollection<PostedTweet>(_config.GetSection("TweetAppDDSettings").GetSection("Collections").GetValue<string>("TweetsCollection"));

        public IMongoCollection<UserInterests> GetUserInterestsCollection() =>
           _mongoDatabase.GetCollection<UserInterests>(_config.GetSection("TweetAppDDSettings").GetSection("Collections").GetValue<string>("UserinterestCollection"));

    }
}
