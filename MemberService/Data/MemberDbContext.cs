
using Microsoft.EntityFrameworkCore;
using MemberService.Models;

namespace MemberService.Data;

public class MemberDbContext : DbContext
{
    public MemberDbContext(DbContextOptions<MemberDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
}
