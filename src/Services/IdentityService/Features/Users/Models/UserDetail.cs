using System;
using System.Collections.Generic;

namespace IdentityService.Features.Users.Models;

public partial class UserDetail
{
    public long UserDetailId { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? CountryCode { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? Dob { get; set; }

    public string? FirstName { get; set; }

    public string? Gender { get; set; }

    public string? ImageUrl { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNo { get; set; }

    public string? PnWithoutCountryCode { get; set; }

    public string? SecretKey { get; set; }

    public string? SocialId { get; set; }

    public int? SocialType { get; set; }

    public string? State { get; set; }

    public string? SuspendReason { get; set; }

    public string? TwoFaType { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? ZipCode { get; set; }

    public string? FacebookSocialId { get; set; }

    public string? InstagramSocialId { get; set; }

    public string? OtherSocialLink { get; set; }

    public string? SampleArtistVideoLinkOne { get; set; }

    public string? SampleArtistVideoLinkTwo { get; set; }

    public string? AboutMe { get; set; }

    public bool? IsBookingEnabled { get; set; }

    public string? Description { get; set; }

    public string? AddressTwo { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
