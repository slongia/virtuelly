namespace IdentityService.Features.Users;

// For list rows
public record AdminUserListItemDto(
    string Id,
    string Email,
    bool EmailConfirmed,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    int AccessFailedCount,
    string[] Roles
);

// For full detail/edit view
public record AdminUserDetailDto(
    string Id,
    string Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    int AccessFailedCount,
    string[] Roles
);

// Create request from admin
public record AdminUserCreateRequest(
    string Email,
    string Password,
    bool EmailConfirmed,
    string? PhoneNumber
);

// Update request from admin
public record AdminUserUpdateRequest(
    string Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd
);

// Response wrapper for paged list
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
