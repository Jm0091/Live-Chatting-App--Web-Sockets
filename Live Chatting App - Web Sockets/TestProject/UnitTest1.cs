using LiveChattingApp;
using LiveChattingApp.Data;
using LiveChattingApp.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace TestProject
namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        //checking username conditions algorithm 
        public async Task TestMethod4_CheckingUsername()
        {
            ChatDBContext _dbContext = Context("method4");
            // arrange
            bool expectedResult = true;

            // act
            ChatHub chathub = new ChatHub(_dbContext);
            bool result = chathub.UsernameInDb("James");

            // assert
            Assert.AreEqual(expectedResult, result, "Checking username in database failed!");
        }

        //Source used of following code structure was provided in my Cnavas: https://buildingsteps.wordpress.com/2018/06/12/testing-signalr-hubs-in-asp-net-core-2-1/
        [TestMethod]
        public async Task TestMethod1()
        {
            ChatDBContext _dbContext = Context("method1");
            // arrange
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            ChatHub simpleHub = new ChatHub(_dbContext)
            {
                Clients = mockClients.Object
            };

            // act
            ChatHub chathub = new ChatHub(_dbContext);
            await chathub.SendAllMsgsAsync("James", "Canada is good!");

            // assert
            mockClients.Verify(clients => clients.All, Times.Once);

            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "welcome",
                    It.Is<object[]>(o => o != null && o.Length == 1 && ((object[])o[0]).Length == 3),
                    default(CancellationToken)),
                Times.Once);
        }

        [TestMethod]
        //checking username conditions algorithm 
        public async Task TestMethod2_CheckingUsername()
        {
            ChatDBContext _dbContext = Context("method2");
            ChatHub chathub = new ChatHub(_dbContext);

            // arrange
            bool expectedResult = false;

            // act
            bool result = chathub.CheckUsername("Bob");

            // assert
            Assert.AreEqual(expectedResult, result, "Checking username failed!");
        }


        [TestMethod]
        //checking message conditions algorithm 
        public async Task TestMethod3_CheckingMessage()
        {
            ChatDBContext _dbContext = Context("method3");
            ChatHub chathub = new ChatHub(_dbContext);

            // arrange
            bool expectedResult = false;

            // act
            bool result = chathub.CheckMessage("https://buildingsteps.wordpress.com/2018/06/12/testing-signalr-hubs-in-asp-net-core-2-1/");

            // assert
            Assert.AreEqual(expectedResult, result, "Checking message condition failed!");
        }

        /// <summary>
        ///  Creating database in memory for testing purpose 
        /// </summary>
        /// <param name="databaseName">Temporary Database name</param>
        /// <returns>context</returns>
        private ChatDBContext Context(string dbName)
        {
            var options = new DbContextOptionsBuilder<ChatDBContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new ChatDBContext(options);
            DateTimeOffset time = DateTimeOffset.Now;
            context.Message.AddRange(
                new Message()
                {
                    MsgId = Guid.NewGuid(),
                    UserName = "James",
                    Msg = "Canada is good!",
                    MsgTime = time,
                }
            );

            context.UserProfile.AddRange(
                new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    UserName = "James",
                    UserCreationTime = time,
                }
            );

            context.ConnectivityTime.AddRange(
                new ConnectivityTime()
                {
                    TimeId = Guid.NewGuid(),
                    UserName = "James",
                    ConnectingTime = time,
                    DisconnectingTime = DateTimeOffset.Now,
                }
            );

            context.SaveChanges();
            return context;
        }
    }
}
