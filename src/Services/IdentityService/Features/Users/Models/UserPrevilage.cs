using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class UserPrevilage
{
    public long UserUserId { get; set; }

    public string Previlage { get; set; } = null!;
}
