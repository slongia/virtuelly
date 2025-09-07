namespace Application.Dtos;

public class AuthResultDto
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
}