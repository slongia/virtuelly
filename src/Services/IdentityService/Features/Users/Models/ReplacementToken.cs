using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class ReplacementToken
{
    public long TokenId { get; set; }

    public string? TokenName { get; set; }

    public string? TokenValue { get; set; }
}
