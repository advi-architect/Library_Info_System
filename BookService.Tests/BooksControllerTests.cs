using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookService.Controllers;
using BookService.Services;
using BookService.DTOs;

namespace BookService.Tests
{
    public class BookControllerTests
    {
        private BooksController CreateController(
            Mock<IBookService> mockService,
            string username,
            string role)
        {
            var controller = new BooksController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        // -------------------------------
        // AddBook
        // -------------------------------

        [Fact]
        public async Task AddBook_Should_Return_Ok()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.AddBookAsync(It.IsAny<AddBookRequest>()))
                .ReturnsAsync(new BookResponse { Id = 1, Title = "Clean Code" });

            var controller = CreateController(mockService, "admin", "Admin");

            var request = new AddBookRequest
            {
                Title = "Clean Code",
                Author = "Robert Martin",
                Genre = "Programming"
            };

            var result = await controller.AddBook(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BookResponse>(okResult.Value);

            Assert.Equal("Clean Code", response.Title);
        }

        // -------------------------------
        // UpdateBook
        // -------------------------------

        [Fact]
        public async Task UpdateBook_Should_Return_NotFound_When_Null()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.UpdateBookAsync(99, It.IsAny<UpdateBookRequest>()))
                .ReturnsAsync((BookResponse)null);

            var controller = CreateController(mockService, "admin", "Admin");

            var result = await controller.UpdateBook(99, new UpdateBookRequest());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateBook_Should_Return_Ok_When_Valid()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.UpdateBookAsync(1, It.IsAny<UpdateBookRequest>()))
                .ReturnsAsync(new BookResponse { Id = 1, Title = "Updated" });

            var controller = CreateController(mockService, "admin", "Admin");

            var result = await controller.UpdateBook(1, new UpdateBookRequest());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BookResponse>(ok.Value);
        }

        // -------------------------------
        // DeleteBook
        // -------------------------------

        [Fact]
        public async Task DeleteBook_Should_Return_NotFound_When_False()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.DeleteBookAsync(1))
                .ReturnsAsync(false);

            var controller = CreateController(mockService, "admin", "Admin");

            var result = await controller.DeleteBook(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBook_Should_Return_Ok_When_True()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.DeleteBookAsync(1))
                .ReturnsAsync(true);

            var controller = CreateController(mockService, "admin", "Admin");

            var result = await controller.DeleteBook(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // -------------------------------
        // Search
        // -------------------------------

        [Fact]
        public async Task Search_Should_Return_List()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.SearchAsync("John", null, 1,5))
                .ReturnsAsync(new List<BookResponse>
                {
                    new BookResponse { Id = 1, Author = "John" }
                });

            var controller = CreateController(mockService, "member", "Member");

            var result = await controller.Search("John",null,1,5);

            var ok = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsAssignableFrom<List<BookResponse>>(ok.Value);

            Assert.Single(books);
        }

        // -------------------------------
        // ReserveBook
        // -------------------------------

        [Fact]
        public async Task ReserveBook_Should_Return_BadRequest_When_False()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.ReserveBookAsync(1, "john"))
                .ReturnsAsync(false);

            var controller = CreateController(mockService, "john", "Member");

            var result = await controller.ReserveBook(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ReserveBook_Should_Return_Ok_When_True()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.ReserveBookAsync(1, "john"))
                .ReturnsAsync(true);

            var controller = CreateController(mockService, "john", "Member");

            var result = await controller.ReserveBook(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // -------------------------------
        // GetMyBooks
        // -------------------------------

        [Fact]
        public async Task GetMyBooks_Should_Return_User_Books()
        {
            var mockService = new Mock<IBookService>();

            mockService.Setup(s => s.GetMyBooksAsync("john"))
                .ReturnsAsync(new List<BookResponse>
                {
                    new BookResponse { Id = 1, Title = "MyBook" }
                });

            var controller = CreateController(mockService, "john", "Member");

            var result = await controller.GetMyBooks();

            var ok = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsAssignableFrom<List<BookResponse>>(ok.Value);

            Assert.Single(books);
        }
    }
}


