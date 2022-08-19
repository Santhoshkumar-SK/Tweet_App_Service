using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_Service.Models
{
    public class PostedTweet
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string TweetId { get; set; }
        public string LoginId { get; set; }
        public string InsightMessage { get; set; }
        public DateTime TimeofPost { get; set; }
        public string TweetMessage { get; set; }
        public int NumberOfLikes { get; set; }
        public List<string> LikedUsers { get; set; }
        public bool IsReplyFlag { get; set; }
        public List<string> RepliedTweetIds { get; set; }
    }
}
