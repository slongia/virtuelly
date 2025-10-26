using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class User
{
    public long UserId { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? Email { get; set; }

    public DateTime? EmailVerificationTime { get; set; }

    public string? Password { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? UserStatus { get; set; }

    public long? FkUserRole { get; set; }

    public long? FkUserDetail { get; set; }

    public string? ApprovalStatus { get; set; }

    public string? ProfileRejectionReason { get; set; }

    public string? CompanyName { get; set; }

    public virtual ICollection<DeviceTokenDetail> DeviceTokenDetails { get; set; } = new List<DeviceTokenDetail>();

    public virtual UserDetail? FkUserDetailNavigation { get; set; }

    public virtual Role? FkUserRoleNavigation { get; set; }

    public virtual ICollection<InvitesTokenDetail> InvitesTokenDetails { get; set; } = new List<InvitesTokenDetail>();

    public virtual ICollection<SmsDetail> SmsDetails { get; set; } = new List<SmsDetail>();

    public virtual ICollection<TokenDetail> TokenDetails { get; set; } = new List<TokenDetail>();

    public virtual ICollection<UserLoginDetail> UserLoginDetails { get; set; } = new List<UserLoginDetail>();

    public virtual ICollection<UserSecurityDetail> UserSecurityDetails { get; set; } = new List<UserSecurityDetail>();
}
