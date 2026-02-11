
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MemberService.Data;
//using MemberService.Models;
//using MemberService.Services;
//using System.Threading.Tasks;


//namespace MemberService.Controllers;

//[ApiController]
//[Route("api/auth")]
//public class AuthController : ControllerBase
//{
//    private readonly MemberDbContext _context;
//    private readonly TokenService _tokenService;

//    public AuthController(MemberDbContext context, TokenService tokenService)
//    {
//        _context = context;
//        _tokenService = tokenService;
//    }

//    [HttpPost("register")]
//    public async Task<IActionResult> Register(User user)
//    {
//        if (await _context.Users.AnyAsync(x => x.Username == user.Username))
//            return BadRequest("User already exists");

//        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
//        _context.Users.Add(user);
//        await _context.SaveChangesAsync();
//        return Ok();
//    }

//    [HttpPost("login")]
//    public async Task<IActionResult> Login(User request)
//    {
//        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
//        if (user == null || !BCrypt.Net.BCrypt.Verify(request.PasswordHash, user.PasswordHash))
//            return Unauthorized();

//        var token = _tokenService.GenerateToken(user);
//        return Ok(new { token });
//    }
//}
using Microsoft.AspNetCore.Mvc;
using MemberService.DTOs;
using MemberService.Services;
using System.Threading.Tasks;

namespace MemberService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMemberService _memberService;

    public AuthController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var success = await _memberService.RegisterAsync(request);

        if (!success)
            return BadRequest("User already exists");

        return Ok("Registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _memberService.LoginAsync(request);

        if (response == null)
            return Unauthorized();

        return Ok(response);
    }
}
