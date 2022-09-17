using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.tweetinfoservice.Models;

namespace com.tweetapp.tweetinfoservice.DataContext
{
    public interface IDBContext
    {      
        public IMongoCollection<PostedTweet> GetTweetsCollection();
        public IMongoCollection<UserInterests> GetUserInterestsCollection();
    }
}
