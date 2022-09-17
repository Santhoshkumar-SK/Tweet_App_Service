using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.userinfoservice.Models
{
    public class KafkaProducerData
    {
        public string UserName { get; set; }
        public List<string> UserInterests { get; set; }
    }
}
