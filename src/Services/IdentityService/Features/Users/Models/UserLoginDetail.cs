using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class UserLoginDetail
{
    public long UserLoginId { get; set; }

    public string? BrowserPrint { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? IpAddress { get; set; }

    public string? Location { get; set; }

    public string? UserAgent { get; set; }

    public long? FkUserId { get; set; }

    public virtual User? FkUser { get; set; }
}
