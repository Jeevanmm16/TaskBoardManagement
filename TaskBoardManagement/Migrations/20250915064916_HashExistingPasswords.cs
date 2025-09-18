using BCrypt.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBoardManagement.Migrations
{
    public partial class HashExistingPasswords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Example for one known user
            var hashed = BCrypt.Net.BCrypt.HashPassword("@Jeevan123");

            migrationBuilder.Sql($@"
                UPDATE [Users]
                SET PasswordHash = '{hashed}'
                WHERE Email = 'user@example.com';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Optional rollback
        }
    }
}
