using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tweet_App_Service.DataContext;
using Tweet_App_Service.Models;
using Tweet_App_Service.Repositories;

namespace Tweet_App_Service_Test.Repositories
{
    [TestFixture]
    public class AuthenticationTest
    {
        private readonly Mock<IDBContext> _mockDbContext;
        private readonly IConfigurationRoot _mockConfiguration;
        private readonly Fixture _fixture;
        private AuthenticationRepo _authRepoForTest;

        

        public AuthenticationTest()
        {
            _mockConfiguration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            _mockDbContext = new Mock<IDBContext>();
            _fixture = new Fixture();
            _authRepoForTest = new AuthenticationRepo(_mockDbContext.Object, _mockConfiguration);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Description("AuthenticationRepo.UserLogin() - Success when gave valid input")]
        [Category("Unit")]
        public void UserLogin_Success()
        {
            var input = _fixture.Build<LoginDTO>().Create();

            
        }


    }
}
