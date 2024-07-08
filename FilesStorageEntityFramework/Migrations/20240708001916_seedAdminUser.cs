using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilesStorageEntityFramework.Migrations
{
    public partial class seedAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var adminRoleId = Guid.NewGuid().ToString();
            var adminUserId = Guid.NewGuid().ToString();
            var adminEmail = "admin@gmail.com";
            var adminPassword = "P@ssw0rd123";
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var hashedPassword = passwordHasher.HashPassword(null, adminPassword);

            // Insert Admin Role
            migrationBuilder.Sql($@"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
            VALUES ('{adminRoleId}', 'Admin', 'ADMIN', '{Guid.NewGuid()}')");

            // Insert Admin User
            migrationBuilder.Sql($@"
            INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount) 
            VALUES ('{adminUserId}', '{adminEmail}', '{adminEmail.ToUpper()}', '{adminEmail}', '{adminEmail.ToUpper()}', 1, '{hashedPassword}', '', '{Guid.NewGuid()}', 0, 0, 0, 0)");

            // Assign Admin User to Admin Role
            migrationBuilder.Sql($@"
            INSERT INTO AspNetUserRoles (UserId, RoleId) 
            VALUES ('{adminUserId}', '{adminRoleId}')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete the Admin User and Role
            migrationBuilder.Sql("DELETE FROM AspNetUserRoles WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE UserName = 'admin@gmail.com')");
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE UserName = 'admin@gmail.com'");
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Name = 'Admin'");

        }
    }
}
