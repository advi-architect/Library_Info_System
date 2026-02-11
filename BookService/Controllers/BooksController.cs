using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookService.Services;
using BookService.DTOs;
using System.Threading.Tasks;

namespace BookService.Controllers;

[ApiController]
[Route("api/books")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // Admin - Add Book
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddBook(AddBookRequest request)
    {
        var result = await _bookService.AddBookAsync(request);
        return Ok(result);
    }

    // Admin - Update Book
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookRequest request)
    {
        var result = await _bookService.UpdateBookAsync(id, request);

        if (result == null)
            return NotFound("Book not found");

        return Ok(result);
    }

    // Admin - Delete Book
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var deleted = await _bookService.DeleteBookAsync(id);

        if (!deleted)
            return NotFound("Book not found");

        return Ok("Book deleted successfully");
    }

    // Member - Search Books
    [HttpGet("search")]
    public async Task<IActionResult> Search(string author, string genre, int page,int pagesize)
    {
        var result = await _bookService.SearchAsync(author, genre, page, pagesize);
        return Ok(result);
    }

    // Member - Reserve Book
    [HttpPost("{id}/reserve")]
    [Authorize(Roles = "Member")]
    public async Task<IActionResult> ReserveBook(int id)
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var success = await _bookService.ReserveBookAsync(id, username);

        if (!success)
            return BadRequest("Unable to reserve book");

        return Ok("Book reserved successfully");
    }

    // Member - Get My Reserved Books
    [HttpGet("my")]
    [Authorize(Roles = "Member")]
    public async Task<IActionResult> GetMyBooks()
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var books = await _bookService.GetMyBooksAsync(username);
        return Ok(books);
    }
}

