//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;
//using Microsoft.EntityFrameworkCore;
//using MemberService.Data;
//using MemberService.Models;
//using MemberService.Controllers;
//using MemberService.Services;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;

//namespace MemberService.Tests
//{

//        public class ReservationControllerTests
//        {
//            private ReservationController CreateController(
//                MemberDbContext context,
//                string username = "john")
//            {
//                var controller = new ReservationController(context);

//                if (username != null)
//                {
//                    var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//                    {
//                new Claim(ClaimTypes.Name, username),
//                new Claim(ClaimTypes.Role, "Member")
//            }, "mock"));

//                    controller.ControllerContext = new ControllerContext
//                    {
//                        HttpContext = new DefaultHttpContext
//                        {
//                            User = user
//                        }
//                    };
//                }

//                return controller;
//            }

//            private MemberDbContext GetInMemoryDb()
//            {
//                var options = new DbContextOptionsBuilder<MemberDbContext>()
//                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                    .Options;

//                return new MemberDbContext(options);
//            }

//            [Fact]
//            public async Task ReserveBook_Should_Return_Ok_When_New_Reservation()
//            {
//                var context = GetInMemoryDb();
//                var controller = CreateController(context);

//                var result = await controller.ReserveBook(1);

//                var okResult = Assert.IsType<OkObjectResult>(result);
//                Assert.Equal("Book reserved successfully.", okResult.Value);

//                Assert.Single(context.Reservations);
//            }

//            [Fact]
//            public async Task ReserveBook_Should_Return_BadRequest_When_Duplicate()
//            {
//                var context = GetInMemoryDb();
//                var controller = CreateController(context);

//                await controller.ReserveBook(1); // first reservation

//                var result = await controller.ReserveBook(1); // duplicate

//                var badResult = Assert.IsType<BadRequestObjectResult>(result);
//                Assert.Equal("You have already reserved this book.", badResult.Value);
//            }

//            [Fact]
//        //public async Task ReserveBook_Should_Return_Unauthorized_When_User_Not_Logged_In()
//        //{
//        //    var context = GetInMemoryDb();
//        //    var controller = CreateController(context, null); // no user

//        //    var result = await controller.ReserveBook(1);

//        //    Assert.IsType<UnauthorizedResult>(result);
//        //}
//        public async Task ReserveBook_Should_Return_Unauthorized_When_User_Not_Logged_In()
//        {
//            var context = GetInMemoryDb();
//            var controller = new ReservationController(context);

//            controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext() // No user assigned
//            };

//            var result = await controller.ReserveBook(1);

//            Assert.IsType<OkObjectResult>(result);
//        }

//        [Fact]
//            public async Task GetMyReservations_Should_Return_Only_Current_User_Reservations()
//            {
//                var context = GetInMemoryDb();

//                // Seed reservations manually
//                context.Reservations.Add(new Reservation
//                {
//                    BookId = 1,
//                    Username = "john"
//                });

//                context.Reservations.Add(new Reservation
//                {
//                    BookId = 2,
//                    Username = "otherUser"
//                });

//                await context.SaveChangesAsync();

//                var controller = CreateController(context, "john");

//                var result = await controller.GetMyReservations();

//                var okResult = Assert.IsType<OkObjectResult>(result);
//                var reservations = Assert.IsAssignableFrom<System.Collections.Generic.List<Reservation>>(okResult.Value);

//                Assert.Single(reservations);
//                Assert.Equal("john", reservations.First().Username);
//            }
//        }

//}

using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MemberService.Controllers;
using MemberService.Services;
using MemberService.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberService.Tests
{
    public class ReservationControllerTests
    {
        private readonly Mock<IMemberService> _mockService;
        private readonly ReservationController _controller;

        public ReservationControllerTests()
        {
            _mockService = new Mock<IMemberService>();
            _controller = new ReservationController(_mockService.Object);
        }

        private void SetupUser(string username)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Member")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // -------------------------
        // RESERVE BOOK
        // -------------------------

        [Fact]
        public async Task ReserveBook_Should_Return_Ok_When_Success()
        {
            SetupUser("john");

            _mockService.Setup(x => x.ReserveBookAsync(1, "john"))
                        .ReturnsAsync(true);

            var result = await _controller.ReserveBook(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReserveBook_Should_Return_BadRequest_When_Duplicate()
        {
            SetupUser("john");

            _mockService.Setup(x => x.ReserveBookAsync(1, "john"))
                        .ReturnsAsync(false);

            var result = await _controller.ReserveBook(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ReserveBook_Should_Return_Unauthorized_When_No_User()
        {
            var result = await _controller.ReserveBook(1);

            Assert.IsType<UnauthorizedResult>(result);
        }

        // -------------------------
        // GET MY RESERVATIONS
        // -------------------------

        [Fact]
        public async Task GetMyReservations_Should_Return_List()
        {
            SetupUser("john");

            var reservations = new List<ReservationResponse>
            {
                new ReservationResponse
                {
                    BookId = 1,
                    Username = "john"
                }
            };

            _mockService.Setup(x => x.GetMyReservationsAsync("john"))
                        .ReturnsAsync(reservations);

            var result = await _controller.GetMyReservations();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<ReservationResponse>>(okResult.Value);

            Assert.Single(value);
        }
    }
}

