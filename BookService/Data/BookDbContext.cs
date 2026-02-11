
using Microsoft.EntityFrameworkCore;
using BookService.Models;

namespace BookService.Data;

public class BookDbContext : DbContext
{
    public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }
    public DbSet<Models.Book> Books => Set<Models.Book>();
    public DbSet<Models.Reservation> Reservations => Set<Models.Reservation>();
}


