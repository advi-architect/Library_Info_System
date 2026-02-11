//using MemberService.Data;
//using MemberService.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MemberService.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class ReservationController : ControllerBase
//{
//    private readonly MemberDbContext _context;

//    public ReservationController(MemberDbContext context)
//    {
//        _context = context;
//    }

//    // MEMBER ONLY - Reserve a book
//    [HttpPost("{bookId}")]
//    [Authorize(Roles = "Member")]
//    public async Task<IActionResult> ReserveBook(int bookId)
//    {
//        var username = "john";

//        if (string.IsNullOrEmpty(username))
//            return Unauthorized();

//        // 🔹 Check duplicate reservation
//        var existingReservation = await _context.Reservations
//            .FirstOrDefaultAsync(r => r.BookId == bookId && r.Username == username);

//        if (existingReservation != null)
//            return BadRequest("You have already reserved this book.");

//        var reservation = new Reservation
//        {
//            BookId = bookId,
//            Username = username,
//            ReservedAt = DateTime.UtcNow
//        };

//        _context.Reservations.Add(reservation);
//        await _context.SaveChangesAsync();

//        return Ok("Book reserved successfully.");
//    }

//    // View My Reservations (Member)
//    [HttpGet("my")]
//    [Authorize(Roles = "Member")]
//    public async Task<IActionResult> GetMyReservations()
//    {
//        var username = User.Identity?.Name;

//        var reservations = await _context.Reservations
//            .Where(r => r.Username == username)
//            .ToListAsync();

//        return Ok(reservations);
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemberService.Services;
using System.Threading.Tasks;

namespace MemberService.Controllers;

[ApiController]
[Route("api/reservations")]
[Authorize(Roles = "Member")]
public class ReservationController : ControllerBase
{
    private readonly IMemberService _memberService;

    public ReservationController(IMemberService memberService)
    {
        _memberService = memberService;
    }
    [HttpPost("{bookId}")]
    public async Task<IActionResult> ReserveBook(int bookId)
    {
        var username = HttpContext?.User?.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var success = await _memberService.ReserveBookAsync(bookId, username);

        if (!success)
            return BadRequest("Already reserved");

        return Ok("Book reserved successfully");
    }


    [HttpGet("my")]
    public async Task<IActionResult> GetMyReservations()
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var reservations = await _memberService
            .GetMyReservationsAsync(username);

        return Ok(reservations);
    }
}

