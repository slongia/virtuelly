using System;
using System.Collections.Generic;
using IdentityService.Features.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Users.Data;

public partial class IdentityDbContext : DbContext
{
    public IdentityDbContext()
    {
    }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DeviceTokenDetail> DeviceTokenDetails { get; set; }

    public virtual DbSet<InvitesTokenDetail> InvitesTokenDetails { get; set; }

    public virtual DbSet<ReplacementToken> ReplacementTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SmsDetail> SmsDetails { get; set; }

    public virtual DbSet<TokenDetail> TokenDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserLoginDetail> UserLoginDetails { get; set; }

    public virtual DbSet<UserPrevilage> UserPrevilages { get; set; }

    public virtual DbSet<UserSecurityDetail> UserSecurityDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=virtuelly;Username=dev;Password=dev@123456");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceTokenDetail>(entity =>
        {
            entity.HasKey(e => e.DeviceTokenId).HasName("device_token_details_pkey");

            entity.ToTable("device_token_details", "identity");

            entity.HasIndex(e => e.FkUserId, "ix_device_token_fk_user");

            entity.Property(e => e.DeviceTokenId)
                .ValueGeneratedNever()
                .HasColumnName("device_token_id");
            entity.Property(e => e.DeviceToken)
                .HasMaxLength(255)
                .HasColumnName("device_token");
            entity.Property(e => e.DeviceType)
                .HasMaxLength(255)
                .HasColumnName("device_type");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");

            entity.HasOne(d => d.FkUser).WithMany(p => p.DeviceTokenDetails)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_device_token_user");
        });

        modelBuilder.Entity<InvitesTokenDetail>(entity =>
        {
            entity.HasKey(e => e.InvitesTokenId).HasName("invites_token_details_pkey");

            entity.ToTable("invites_token_details", "identity");

            entity.HasIndex(e => e.UserId, "ix_invite_token_user");

            entity.Property(e => e.InvitesTokenId)
                .ValueGeneratedNever()
                .HasColumnName("invites_token_id");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.InvitesTokenDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_invite_token_user");
        });

        modelBuilder.Entity<ReplacementToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("replacement_token_pkey");

            entity.ToTable("replacement_token", "identity");

            entity.Property(e => e.TokenId)
                .ValueGeneratedNever()
                .HasColumnName("token_id");
            entity.Property(e => e.TokenName)
                .HasMaxLength(255)
                .HasColumnName("token_name");
            entity.Property(e => e.TokenValue)
                .HasMaxLength(255)
                .HasColumnName("token_value");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");

            entity.ToTable("role", "identity");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.Role1)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<SmsDetail>(entity =>
        {
            entity.HasKey(e => e.OtpId).HasName("sms_details_pkey");

            entity.ToTable("sms_details", "identity");

            entity.HasIndex(e => e.UserId, "ix_sms_user");

            entity.Property(e => e.OtpId)
                .ValueGeneratedNever()
                .HasColumnName("otp_id");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Otp).HasColumnName("otp");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.SmsDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sms_user");
        });

        modelBuilder.Entity<TokenDetail>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("token_details_pkey");

            entity.ToTable("token_details", "identity");

            entity.HasIndex(e => e.UserId, "ix_token_user");

            entity.Property(e => e.TokenId)
                .ValueGeneratedNever()
                .HasColumnName("token_id");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(255)
                .HasColumnName("ip_address");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.TokenDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_token_user");
        });

        modelBuilder.Entity<Models.User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.ToTable("user", "identity");

            entity.HasIndex(e => e.FkUserDetail, "ix_user_fk_user_detail");

            entity.HasIndex(e => e.FkUserRole, "ix_user_fk_user_role");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(32)
                .HasColumnName("approval_status");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerificationTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("email_verification_time");
            entity.Property(e => e.FkUserDetail).HasColumnName("fk_user_detail");
            entity.Property(e => e.FkUserRole).HasColumnName("fk_user_role");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.ProfileRejectionReason).HasColumnName("profile_rejection_reason");
            entity.Property(e => e.UpdateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_time");
            entity.Property(e => e.UserStatus)
                .HasMaxLength(32)
                .HasColumnName("user_status");

            entity.HasOne(d => d.FkUserDetailNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkUserDetail)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_detail");

            entity.HasOne(d => d.FkUserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkUserRole)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_role");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserDetailId).HasName("user_detail_pkey");

            entity.ToTable("user_detail", "identity");

            entity.Property(e => e.UserDetailId)
                .ValueGeneratedNever()
                .HasColumnName("user_detail_id");
            entity.Property(e => e.AboutMe).HasColumnName("about_me");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AddressTwo).HasColumnName("address_two");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasColumnName("country");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(255)
                .HasColumnName("country_code");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Dob)
                .HasMaxLength(255)
                .HasColumnName("dob");
            entity.Property(e => e.FacebookSocialId).HasColumnName("facebook_social_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasColumnName("gender");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.InstagramSocialId).HasColumnName("instagram_social_id");
            entity.Property(e => e.IsBookingEnabled).HasColumnName("is_booking_enabled");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.OtherSocialLink).HasColumnName("other_social_link");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(255)
                .HasColumnName("phone_no");
            entity.Property(e => e.PnWithoutCountryCode)
                .HasMaxLength(255)
                .HasColumnName("pn_without_country_code");
            entity.Property(e => e.SampleArtistVideoLinkOne).HasColumnName("sample_artist_video_link_one");
            entity.Property(e => e.SampleArtistVideoLinkTwo).HasColumnName("sample_artist_video_link_two");
            entity.Property(e => e.SecretKey)
                .HasMaxLength(255)
                .HasColumnName("secret_key");
            entity.Property(e => e.SocialId)
                .HasMaxLength(255)
                .HasColumnName("social_id");
            entity.Property(e => e.SocialType).HasColumnName("social_type");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
            entity.Property(e => e.SuspendReason).HasColumnName("suspend_reason");
            entity.Property(e => e.TwoFaType)
                .HasMaxLength(32)
                .HasColumnName("two_fa_type");
            entity.Property(e => e.UpdateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_time");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(255)
                .HasColumnName("zip_code");
        });

        modelBuilder.Entity<UserLoginDetail>(entity =>
        {
            entity.HasKey(e => e.UserLoginId).HasName("user_login_detail_pkey");

            entity.ToTable("user_login_detail", "identity");

            entity.HasIndex(e => e.FkUserId, "ix_user_login_fk_user");

            entity.Property(e => e.UserLoginId)
                .ValueGeneratedNever()
                .HasColumnName("user_login_id");
            entity.Property(e => e.BrowserPrint)
                .HasMaxLength(255)
                .HasColumnName("browser_print");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(255)
                .HasColumnName("ip_address");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255)
                .HasColumnName("user_agent");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserLoginDetails)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_login_user");
        });

        modelBuilder.Entity<UserPrevilage>(entity =>
        {
            entity.HasKey(e => new { e.UserUserId, e.Previlage }).HasName("user_previlage_pkey");

            entity.ToTable("user_previlage", "identity");

            entity.Property(e => e.UserUserId).HasColumnName("user_user_id");
            entity.Property(e => e.Previlage)
                .HasMaxLength(255)
                .HasColumnName("previlage");
        });

        modelBuilder.Entity<UserSecurityDetail>(entity =>
        {
            entity.HasKey(e => e.UserSecurityDetailsId).HasName("user_security_details_pkey");

            entity.ToTable("user_security_details", "identity");

            entity.HasIndex(e => e.FkUserId, "ix_user_security_fk_user");

            entity.Property(e => e.UserSecurityDetailsId)
                .ValueGeneratedNever()
                .HasColumnName("user_security_details_id");
            entity.Property(e => e.AcitvityMessage)
                .HasMaxLength(255)
                .HasColumnName("acitvity_message");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.IpAddess)
                .HasMaxLength(255)
                .HasColumnName("ip_addess");
            entity.Property(e => e.Source)
                .HasMaxLength(255)
                .HasColumnName("source");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserSecurityDetails)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_security_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
