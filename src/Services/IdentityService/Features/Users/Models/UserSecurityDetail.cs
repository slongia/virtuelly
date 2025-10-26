using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class UserSecurityDetail
{
    public long UserSecurityDetailsId { get; set; }

    public string? AcitvityMessage { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? IpAddess { get; set; }

    public string? Source { get; set; }

    public string? Status { get; set; }

    public long? FkUserId { get; set; }

    public virtual User? FkUser { get; set; }
}
