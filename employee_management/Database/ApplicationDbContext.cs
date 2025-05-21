using employee_management.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace employee_management.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Seed Roles
            var adminRoleId = "1";
            var userRoleId = "2";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            // 2. Create default Admin user
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminId = "a1";

            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminId,
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            builder.Entity<ApplicationUser>().HasData(adminUser);

            // 3. Assign Admin role to user
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminId
                }
            );

        }

    }
}
