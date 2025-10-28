-- Ensure schema exists
CREATE SCHEMA IF NOT EXISTS identity;

-- Users table
CREATE TABLE IF NOT EXISTS identity."AspNetUsers" (
    "Id"                TEXT PRIMARY KEY,
    "UserName"          TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email"             TEXT NULL,
    "NormalizedEmail"   TEXT NULL,
    "EmailConfirmed"    BOOLEAN NOT NULL DEFAULT FALSE,
    "PasswordHash"      TEXT NULL,
    "SecurityStamp"     TEXT NULL,
    "ConcurrencyStamp"  TEXT NULL,
    "PhoneNumber"       TEXT NULL,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "TwoFactorEnabled"  BOOLEAN NOT NULL DEFAULT FALSE,
    "LockoutEnd"        TIMESTAMPTZ NULL,
    "LockoutEnabled"    BOOLEAN NOT NULL DEFAULT FALSE,
    "AccessFailedCount" INTEGER NOT NULL DEFAULT 0
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_AspNetUsers_NormalizedUserName"
ON identity."AspNetUsers"("NormalizedUserName");

CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_NormalizedEmail"
ON identity."AspNetUsers"("NormalizedEmail");

-- Roles table
CREATE TABLE IF NOT EXISTS identity."AspNetRoles" (
    "Id"               TEXT PRIMARY KEY,
    "Name"             TEXT NULL,
    "NormalizedName"   TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_AspNetRoles_NormalizedName"
ON identity."AspNetRoles"("NormalizedName");

-- UserRoles (many-to-many join)
CREATE TABLE IF NOT EXISTS identity."AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    PRIMARY KEY ("UserId","RoleId"),
    CONSTRAINT "FK_UserRoles_Users"
        FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserRoles_Roles"
        FOREIGN KEY ("RoleId") REFERENCES identity."AspNetRoles"("Id") ON DELETE CASCADE
);

-- UserClaims
CREATE TABLE IF NOT EXISTS identity."AspNetUserClaims" (
    "Id"        SERIAL PRIMARY KEY,
    "UserId"    TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_UserClaims_Users"
        FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId"
ON identity."AspNetUserClaims"("UserId");

-- RoleClaims
CREATE TABLE IF NOT EXISTS identity."AspNetRoleClaims" (
    "Id"         SERIAL PRIMARY KEY,
    "RoleId"     TEXT NOT NULL,
    "ClaimType"  TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_RoleClaims_Roles"
        FOREIGN KEY ("RoleId") REFERENCES identity."AspNetRoles"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId"
ON identity."AspNetRoleClaims"("RoleId");

-- UserLogins (external providers like Google / Facebook / Apple)
CREATE TABLE IF NOT EXISTS identity."AspNetUserLogins" (
    "LoginProvider"        TEXT NOT NULL,
    "ProviderKey"          TEXT NOT NULL,
    "ProviderDisplayName"  TEXT NULL,
    "UserId"               TEXT NOT NULL,
    PRIMARY KEY ("LoginProvider","ProviderKey"),
    CONSTRAINT "FK_UserLogins_Users"
        FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId"
ON identity."AspNetUserLogins"("UserId");

-- UserTokens (for refresh tokens / recovery codes etc.)
CREATE TABLE IF NOT EXISTS identity."AspNetUserTokens" (
    "UserId"         TEXT NOT NULL,
    "LoginProvider"  TEXT NOT NULL,
    "Name"           TEXT NOT NULL,
    "Value"          TEXT NULL,
    PRIMARY KEY ("UserId","LoginProvider","Name"),
    CONSTRAINT "FK_UserTokens_Users"
        FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE
);
