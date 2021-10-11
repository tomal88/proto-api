using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            SeedUserAndRoles(builder);
            base.OnModelCreating(builder);
        }

        private void SeedUserAndRoles(ModelBuilder builder)
        {
            string userId = "662278bb-5893-4b8c-89f0-592edfacef22";
            string roleId = "8a49e3ac-aa54-4b4c-8e7a-b9f99f67e7f7";

            var adminRole = new IdentityRole()
            {
                Id = roleId,
                Name = UserRole.Admin.ToString(),
                ConcurrencyStamp = "1f900332-556d-4f74-8700-91161c4118b4",
                NormalizedName = UserRole.Admin.ToString().ToUpper()
            };

            var generalRole = new IdentityRole()
            {
                Id = "e5eb009a-767d-4b5e-b3cd-3513063e46c6",
                Name = UserRole.GeneralUser.ToString(),
                ConcurrencyStamp = "380bdb8e-6f39-444f-830e-903a0f3b4482",
                NormalizedName = UserRole.GeneralUser.ToString().ToUpper()
            };

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                LockoutEnabled = false,
                ConcurrencyStamp = "5b2f5a62-691c-4188-99f3-068c158d8677",
                SecurityStamp = "0197e84b-fd4e-4622-956b-870fe6102150",
                PasswordHash = "AQAAAAEAACcQAAAAEJ/t6u7aASZG6DMGaFkLr5XlRW3NRRHWbMpZwWG2FzfLR1k+YwiOgiKuuS+UQEtv+w==" //AdminPass123@#
            };

            builder.Entity<ApplicationUser>().HasData(user);
            builder.Entity<IdentityRole>().HasData(adminRole);
            builder.Entity<IdentityRole>().HasData(generalRole);
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { UserId = userId, RoleId = roleId }
            );
        }
    }
}
