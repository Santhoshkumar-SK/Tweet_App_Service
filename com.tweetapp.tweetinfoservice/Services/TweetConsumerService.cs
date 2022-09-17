using com.tweetapp.tweetinfoservice.DataContext;
using com.tweetapp.tweetinfoservice.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace com.tweetapp.tweetinfoservice.Services
{
    public class TweetConsumerService:BackgroundService
    {
        static readonly log4net.ILog _logging = log4net.LogManager.GetLogger(typeof(TweetConsumerService));
        private ConsumerConfig _consumerconfig;
        private IConfiguration _config;
        private IDBContext _dBContext;
        private IMongoCollection<UserInterests> _userInterestCollection;
        public TweetConsumerService(ConsumerConfig consumerconfig,IConfiguration config,IDBContext dBContext)
        {
            _consumerconfig = consumerconfig;
            _config = config;
            _dBContext = dBContext;
            _userInterestCollection = _dBContext.GetUserInterestsCollection();
        }        

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => Start(stoppingToken));
            return Task.CompletedTask;
        }

        private async Task Start(CancellationToken cancellationToken)
        {
            _logging.Info("Consumer Started consuming");
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                using (var c = new ConsumerBuilder<Null, string>(_consumerconfig).Build())
                {
                    string _topicname = _config.GetSection("KafkaSettings").GetValue<string>("TopicName");
                    c.Subscribe(_topicname);
                    int i = 0;
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        i++;
                        var cr = c.Consume(cts.Token);
                        _logging.Info($"{i} - {DateTime.Now} - {cr.Message.Value}");
                        KafkaConsumerData consumerData = JsonSerializer.Deserialize<KafkaConsumerData>(cr.Message.Value);
                        await InsertUserInterest(new UserInterests() { LoginId = consumerData.UserName,UserInterest = consumerData.UserInterests});
                    }
                }                
            }
            catch (Exception ex)
            {
                _logging.Error($"Exception Occured While Consuming Message from Kafka Exception Message : {ex.Message} Stack Trace : {ex.StackTrace}");
            }            
        }

        private async Task InsertUserInterest(UserInterests consumerData)
        {
            UserInterests existingData = new UserInterests();
            var filter = Builders<UserInterests>.Filter.Regex("LoginId", new BsonRegularExpression(consumerData.LoginId));
            existingData = await _userInterestCollection.Find(filter).FirstOrDefaultAsync();

            if(existingData == null)
            {
                _userInterestCollection.InsertOne(consumerData);
            }
        }
    }
}
