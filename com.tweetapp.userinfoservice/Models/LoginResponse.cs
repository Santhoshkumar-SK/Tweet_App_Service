using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.userinfoservice.Models
{
    public class LoginResponse
    {
        public string LoginMessage { get; set; }
        public string BearerToken { get; set; }
    }
}
