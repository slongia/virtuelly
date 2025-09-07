namespace Core.Entities.Result;

public class AuthResult
{
    public bool Success { get; set; }
    public User? User { get; set; }
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
}