using Microsoft.EntityFrameworkCore;
using TaskBoardManagement.Models.Domain;

namespace TaskBoardManagement.Data
{
     public static class DbInitializer
    {
       
        public static void Seed(AppDbContext context)
        {
          
            // Use migrations instead of EnsureCreated
            context.Database.Migrate();

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role {  Name = "Admin" },
                    new Role {  Name = "Manager" },
                    new Role {  Name = "Developer" }
                );
                context.SaveChanges();
            }

            if (!context.Users.Any(u => u.RoleId == 1))
            {
                var adminId = Guid.NewGuid();

                var admin = new User
                {
                    Id = adminId,
                    Email = "admin@taskboard.com",
                    PasswordHash = "admin123",
                    RoleId = 1
                };

                var adminProfile = new UserProfile
                {
                    UserId = adminId,
                    FullName = "Jeevan m m",
                    Phone = "9999999999",
                    Address = "Davanagere"
                };

                context.Users.Add(admin);
                context.UserProfiles.Add(adminProfile);

                context.SaveChanges();
            }
        }

    }
}



