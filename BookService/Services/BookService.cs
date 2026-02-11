using BookService.Data;
using BookService.DTOs;
using BookService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BookService.Services;

public class BookService : IBookService
{
    private readonly BookDbContext _context;

    public BookService(BookDbContext context)
    {
        _context = context;
    }

    public async Task<BookResponse> AddBookAsync(AddBookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Genre = request.Genre,
            IsAvailable = true
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return Map(book);
    }

    public async Task<BookResponse> UpdateBookAsync(int id,UpdateBookRequest request)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return null;

        book.Title = request.Title;
        book.Author = request.Author;
        book.Genre = request.Genre;
        book.IsAvailable = request.IsAvailable;

        await _context.SaveChangesAsync();
        return Map(book);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<BookResponse>> SearchAsync(string author, string genre, int page,int pagesize)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrEmpty(author))
            query = query.Where(b => b.Author.Contains(author));

        if (!string.IsNullOrEmpty(genre))
            query = query.Where(b => b.Genre.Contains(genre));

        var books = await query
            .Skip((page - 1) * pagesize)
            .Take(pagesize)
            .ToListAsync();

        return books.Select(Map).ToList();
    }

    public async Task<bool> ReserveBookAsync(int id, string username)
    {
        if (await _context.Reservations.AnyAsync(r => r.BookId == id && r.Username == username))
            return false;

        var book = await _context.Books.FindAsync(id);
        if (book == null || !book.IsAvailable)
            return false;

        book.IsAvailable = false;

        _context.Reservations.Add(new Reservation
        {
            BookId = id,
            Username = username
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<BookResponse>> GetMyBooksAsync(string username)
    {
        var books = await _context.Reservations
            .Where(r => r.Username == username)
            .Join(_context.Books,
                r => r.BookId,
                b => b.Id,
                (r, b) => b)
            .ToListAsync();

        return books.Select(Map).ToList();
    }

    private static BookResponse Map(Book book) =>
        new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            IsAvailable = book.IsAvailable
        };
}
