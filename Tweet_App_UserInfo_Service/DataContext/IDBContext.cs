using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_UserInfo_Service.Models;

namespace Tweet_App_UserInfo_Service.DataContext
{
    public interface IDBContext
    {
        public IMongoCollection<UserInfo> GetUserInfoCollection();
    }
}
