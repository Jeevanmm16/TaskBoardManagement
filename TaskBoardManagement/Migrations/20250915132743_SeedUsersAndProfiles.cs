using Microsoft.EntityFrameworkCore.Migrations;
using BCrypt.Net; // Install BCrypt.Net-Next

#nullable disable

namespace TaskBoardManagement.Migrations
{
    public partial class SeedUsersAndProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Predefined roles (ensure these already exist in Roles table!)
            var adminRoleId = 1;
            var managerRoleId = 2;
            var developerRoleId = 3;

            // 10 Users
            var users = new (Guid Id, string Email, string Password, int RoleId)[]
            {
                (Guid.NewGuid(), "admin@taskboard.com", "Admin@123", adminRoleId),
                (Guid.NewGuid(), "manager1@taskboard.com", "Manager@123", managerRoleId),
                (Guid.NewGuid(), "manager2@taskboard.com", "Manager@123", managerRoleId),
                (Guid.NewGuid(), "dev1@taskboard.com", "Dev@123", developerRoleId),
                (Guid.NewGuid(), "dev2@taskboard.com", "Dev@123", developerRoleId),
                (Guid.NewGuid(), "dev3@taskboard.com", "Dev@123", developerRoleId),
                (Guid.NewGuid(), "dev4@taskboard.com", "Dev@123", developerRoleId),
                (Guid.NewGuid(), "dev5@taskboard.com", "Dev@123", developerRoleId),
                (Guid.NewGuid(), "dev6@taskboard.com", "Dev@123", developerRoleId),
                (Guid.NewGuid(), "dev7@taskboard.com", "Dev@123", developerRoleId),
            };

            foreach (var user in users)
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                migrationBuilder.InsertData(
                    table: "Users",
                    columns: new[] { "Id", "Email", "PasswordHash", "RoleId" },
                    values: new object[] { user.Id, user.Email, hash, user.RoleId }
                );

                // Add corresponding profile
                migrationBuilder.InsertData(
                    table: "UserProfiles",
                    columns: new[] { "UserId", "FullName", "Phone", "Address" },
                    values: new object[]
                    {
                        user.Id,
                        user.Email.Split('@')[0].ToUpper(), // simple name
                        "9876543210",
                        "Sample Address for " + user.Email
                    }
                );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete seeded users
            migrationBuilder.Sql("DELETE FROM [UserProfiles]");
            migrationBuilder.Sql("DELETE FROM [Users]");
        }
    }
}
