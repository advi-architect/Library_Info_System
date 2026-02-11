using MemberService.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberService.Services;

public interface IMemberService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> ReserveBookAsync(int bookId, string username);
    Task<List<ReservationResponse>> GetMyReservationsAsync(string username);
}
