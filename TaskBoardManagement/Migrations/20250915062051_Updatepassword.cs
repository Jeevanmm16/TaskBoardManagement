using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskBoardManagement.Data;
using TaskBoardManagement.Models.Domain;

#nullable disable

namespace TaskBoardManagement.Migrations
{
    /// <inheritdoc />
    public partial class Updatepassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Run raw SQL to fetch & update user passwords
            using (var db = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TaskBoardDb;Trusted_Connection=True;TrustServerCertificate=True;"
) // ⚠️ Replace with your connection string
                    .Options))
            {
                var users = db.Users.ToList();

                foreach (var user in users)
                {
                    // If password is not already a BCrypt hash, re-hash it
                    if (!user.PasswordHash.StartsWith("$2a$") &&
                        !user.PasswordHash.StartsWith("$2b$") &&
                        !user.PasswordHash.StartsWith("$2y$"))
                    {
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                    }
                }

                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ❌ Cannot revert to plain-text passwords
        }
    }
}
