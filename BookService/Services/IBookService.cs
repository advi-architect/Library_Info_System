using BookService.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBookService
{
    Task<BookService.DTOs.BookResponse> AddBookAsync(BookService.DTOs.AddBookRequest request);
    Task<BookService.DTOs.BookResponse> UpdateBookAsync(int id, BookService.DTOs.UpdateBookRequest request);
    Task<bool> DeleteBookAsync(int id);
    Task<List<BookService.DTOs.BookResponse>> SearchAsync(string author, string genre, int page,int pagesize);
    Task<bool> ReserveBookAsync(int id, string username);
    Task<List<BookService.DTOs.BookResponse>> GetMyBooksAsync(string username);
}
