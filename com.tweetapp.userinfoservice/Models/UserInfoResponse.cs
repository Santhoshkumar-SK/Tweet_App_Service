using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.userinfoservice.Models
{
    public class UserInfoResponse
    {
        public UserInfo UserInfo { get; set; }
        public string ResponseMessage { get; set; }
    }
}
