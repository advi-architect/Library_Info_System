//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Mvc;
//using MemberService.Controllers;
//using MemberService.Data;
//using MemberService.Models;
//using MemberService.Services;
//using Microsoft.Extensions.Configuration;
//namespace MemberService.Tests
//{
//   public class AuthControllerTests
//    {
//        private AuthController CreateController()
//        {
//            var options = new DbContextOptionsBuilder<MemberDbContext>()
//                .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                .Options;

//            var context = new MemberDbContext(options);

//            var config = new ConfigurationBuilder()
//                .AddInMemoryCollection(new Dictionary<string, string>
//                {
//        {"Jwt:Key", "ThisIsASecretKeyForTesting123456789"},
//        {"Jwt:Issuer", "TestIssuer"},
//        {"Jwt:Audience", "TestAudience"}
//                })
//                .Build();

//            var tokenService = new TokenService(config);

//            //var controller = new AuthController(context, tokenService);

//            //  var tokenService = new FakeTokenService();

//            return new AuthController(context, tokenService);
//        }

//        [Fact]
//        public async Task Register_Should_Return_Ok_When_User_Is_New()
//        {
//            var controller = CreateController();

//            var result = await controller.Register(new User
//            {
//                Username = "john",
//                PasswordHash = "123"
//            });

//            Assert.IsType<OkResult>(result);
//        }

//        [Fact]
//        public async Task Register_Should_Return_BadRequest_When_User_Exists()
//        {
//            var controller = CreateController();

//            await controller.Register(new User
//            {
//                Username = "john",
//                PasswordHash = "123"
//            });

//            var result = await controller.Register(new User
//            {
//                Username = "john",
//                PasswordHash = "123"
//            });

//            Assert.IsType<BadRequestObjectResult>(result);
//        }

//        [Fact]
//        public async Task Login_Should_Return_Token_When_Valid()
//        {
//            var controller = CreateController();

//            await controller.Register(new User
//            {
//                Username = "john",
//                PasswordHash = "123"
//            });

//            var result = await controller.Login(new User
//            {
//                Username = "john",
//                PasswordHash = "123"
//            });

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.NotNull(okResult.Value);
//        }

//        [Fact]
//        public async Task Login_Should_Return_Unauthorized_When_Invalid()
//        {
//            var controller = CreateController();

//            var result = await controller.Login(new User
//            {
//                Username = "invalid",
//                PasswordHash = "123"
//            });

//            Assert.IsType<UnauthorizedResult>(result);
//        }
//    }
//}
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MemberService.Controllers;
using MemberService.Services;
using MemberService.DTOs;
using System.Threading.Tasks;

namespace MemberService.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IMemberService> _mockService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockService = new Mock<IMemberService>();
            _controller = new AuthController(_mockService.Object);
        }

        // -------------------------
        // REGISTER
        // -------------------------

        [Fact]
        public async Task Register_Should_Return_Ok_When_Success()
        {
            var request = new RegisterRequest
            {
                Username = "john",
                Password = "123"
            };

            _mockService.Setup(x => x.RegisterAsync(request))
                        .ReturnsAsync(true);

            var result = await _controller.Register(request);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_Should_Return_BadRequest_When_User_Exists()
        {
            var request = new RegisterRequest
            {
                Username = "john",
                Password = "123"
            };

            _mockService.Setup(x => x.RegisterAsync(request))
                        .ReturnsAsync(false);

            var result = await _controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // -------------------------
        // LOGIN
        // -------------------------

        [Fact]
        public async Task Login_Should_Return_Token_When_Valid()
        {
            var request = new LoginRequest
            {
                Username = "john",
                Password = "123"
            };

            var response = new LoginResponse
            {
                Token = "fake-jwt-token"
            };

            _mockService.Setup(x => x.LoginAsync(request))
                        .ReturnsAsync(response);

            var result = await _controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<LoginResponse>(okResult.Value);

            Assert.Equal("fake-jwt-token", value.Token);
        }

        [Fact]
        public async Task Login_Should_Return_Unauthorized_When_Invalid()
        {
            var request = new LoginRequest
            {
                Username = "john",
                Password = "wrong"
            };

            _mockService.Setup(x => x.LoginAsync(request))
                        .ReturnsAsync((LoginResponse?)null);

            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
