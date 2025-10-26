using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class DeviceTokenDetail
{
    public long DeviceTokenId { get; set; }

    public string? DeviceToken { get; set; }

    public string? DeviceType { get; set; }

    public long? FkUserId { get; set; }

    public virtual User? FkUser { get; set; }
}
