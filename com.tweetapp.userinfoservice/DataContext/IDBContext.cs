using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.tweetapp.userinfoservice.Models;

namespace com.tweetapp.userinfoservice.DataContext
{
    public interface IDBContext
    {
        public IMongoCollection<UserInfo> GetUserInfoCollection();
    }
}
