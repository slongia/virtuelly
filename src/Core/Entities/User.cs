using Microsoft.AspNetCore.Identity;
namespace Core.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    // public string CompanyName { get; set; } = string.Empty;
    // public DateTime CreatedOn { get; set; } = DateTime.MinValue;
    // public DateTime UpdatedOn { get; set; } = DateTime.MinValue;
    // public bool Status { get; set; }
}