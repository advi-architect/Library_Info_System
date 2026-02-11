using System;

namespace MemberService.DTOs;

public class ReservationResponse
{
    public int BookId { get; set; }
    public string Username { get; set; }
    public DateTime ReservedAt { get; set; }
}
