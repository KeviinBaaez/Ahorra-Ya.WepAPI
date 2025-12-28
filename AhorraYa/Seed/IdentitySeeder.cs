using AhorraYa.Entities.MicrosoftIdentity;
using Microsoft.AspNetCore.Identity;

namespace AhorraYa.WebApi.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<Role>>();
            var userManager = service.GetRequiredService<UserManager<User>>();

            const string adminRole = "Admin";
            const string adminUser = "Admin";
            const string adminPassword = "Adm1n!";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = adminRole
                });
            }

            var user = await userManager.FindByNameAsync(adminUser);
            if (user is null)
            {
                user = new User{
                    UserName = adminUser,
                    Email = "admin@ahorraya.com",
                    EmailConfirmed = true,
                    Name = "Administrador"
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if(!await userManager.IsInRoleAsync(user, adminRole))
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
    }
}
