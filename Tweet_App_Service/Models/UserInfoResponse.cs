using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_Service.Models
{
    public class UserInfoResponse
    {
        public UserInfo UserInfo { get; set; }
        public string ResponseMessage { get; set; }
    }
}
