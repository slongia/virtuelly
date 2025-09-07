using System.Runtime.ConstrainedExecution;
using System.Security.AccessControl;

namespace Application.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}