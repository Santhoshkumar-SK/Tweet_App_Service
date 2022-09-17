using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.tweetinfoservice.Models
{
    public class KafkaConsumerData
    {
        public string UserName { get; set; }
        public List<string> UserInterests { get; set; }
    }
}
