using MemberService.Data;
using MemberService.DTOs;
using MemberService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MemberService.Services;

public class MemberManager : IMemberService
{
    private readonly MemberDbContext _context;
    private readonly TokenService _tokenService;

    public MemberManager(MemberDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(x => x.Username == request.Username))
            return false;

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == request.Username);

        if (user == null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token
        };
    }

    public async Task<bool> ReserveBookAsync(int bookId, string username)
    {
        var exists = await _context.Reservations
            .AnyAsync(r => r.BookId == bookId && r.Username == username);

        if (exists)
            return false;

        var reservation = new Reservation
        {
            BookId = bookId,
            Username = username,
            ReservedAt = DateTime.UtcNow
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ReservationResponse>> GetMyReservationsAsync(string username)
    {
        var reservations = await _context.Reservations
            .Where(r => r.Username == username)
            .ToListAsync();

        return reservations.Select(r => new ReservationResponse
        {
            BookId = r.BookId,
            Username = r.Username,
            ReservedAt = r.ReservedAt
        }).ToList();
    }
}
