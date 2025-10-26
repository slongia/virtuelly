using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class SmsDetail
{
    public long OtpId { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? Otp { get; set; }

    public long? UserId { get; set; }

    public virtual User? User { get; set; }
}
