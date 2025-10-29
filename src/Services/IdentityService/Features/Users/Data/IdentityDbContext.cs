using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Users.Data;

// IMPORTANT: this must inherit from IdentityDbContext<IdentityUser, IdentityRole, string, ... >
// so that EF Core knows about IdentityUser, IdentityRole, etc.
public class IdentityAppDbContext
    : IdentityDbContext<
        IdentityUser,
        IdentityRole,
        string,
        IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
{
    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Make sure all ASP.NET Identity tables live in the "identity" schema
        builder.HasDefaultSchema("identity");

        builder.Entity<IdentityUser>(b =>
        {
            b.ToTable("AspNetUsers", "identity");
        });

        builder.Entity<IdentityRole>(b =>
        {
            b.ToTable("AspNetRoles", "identity");
        });

        builder.Entity<IdentityUserRole<string>>(b =>
        {
            b.ToTable("AspNetUserRoles", "identity");
        });

        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.ToTable("AspNetUserClaims", "identity");
        });

        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.ToTable("AspNetUserLogins", "identity");
        });

        builder.Entity<IdentityRoleClaim<string>>(b =>
        {
            b.ToTable("AspNetRoleClaims", "identity");
        });

        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.ToTable("AspNetUserTokens", "identity");
        });
    }
}
