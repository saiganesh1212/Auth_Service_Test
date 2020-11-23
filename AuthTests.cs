using Airline_Reservation.Controllers;
using Airline_Reservation.DAL;
using Airline_Reservation.Models;
using Airline_Reservation.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_Service_Test
{
    public class Tests
    {
        List<User> user = new List<User>();
        IQueryable<User> userdata;
        Mock<DbSet<User>> mockSet;
        Mock<UserDbContext> usercontextmock;
        Mock<IConfiguration> config;
        [SetUp]
        public void Setup()
        {
            user = new List<User>()
            {
                new User{UserId=1,Username="Ganesh",Password="Sai@1212"}

            };
            userdata = user.AsQueryable();
            mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userdata.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userdata.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userdata.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userdata.GetEnumerator());
            var p = new DbContextOptions<UserDbContext>();
            usercontextmock = new Mock<UserDbContext>(p);
            usercontextmock.Setup(x => x.Users).Returns(mockSet.Object);
            config = new Mock<IConfiguration>();
            config.Setup(p => p["Jwt:Key"]).Returns("ThisismySecretKey");
        }
        [Test]
        public void Login_Success_Test()
        {
            
            
            var authrepo = new AuthRepo(usercontextmock.Object);
            var controller = new AuthController( config.Object,authrepo);
            var auth = controller.Login(new User { UserId = 1, Username = "Ganesh", Password = "Sai@1212" }) as OkObjectResult;

            Assert.AreEqual(200, auth.StatusCode);

        }
        [Test]
        public void Login_Failure_Test()
        {
            var authrepo = new AuthRepo(usercontextmock.Object);
            var controller = new AuthController(config.Object, authrepo);
            var auth = controller.Login(new User { UserId = 1, Username = "Suresh", Password = "Sai@1212" }) as OkObjectResult;
            Assert.IsNull(auth);
        }

    }
}