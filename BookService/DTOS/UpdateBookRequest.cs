namespace BookService.DTOs;

public class UpdateBookRequest
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public bool IsAvailable { get; set; }
}
