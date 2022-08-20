using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_Authentication_Service.Models
{
    public class LoginResponse
    {
        public string LoginMessage { get; set; }
        public string BearerToken { get; set; }
    }
}
