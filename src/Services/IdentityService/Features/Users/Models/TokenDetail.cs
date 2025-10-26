using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class TokenDetail
{
    public long TokenId { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? Email { get; set; }

    public string? IpAddress { get; set; }

    public string? Token { get; set; }

    public long? UserId { get; set; }

    public virtual User? User { get; set; }
}
