using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class Role
{
    public long RoleId { get; set; }

    public string? Role1 { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
