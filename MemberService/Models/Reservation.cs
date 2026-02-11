using System;

namespace MemberService.Models;

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
}
