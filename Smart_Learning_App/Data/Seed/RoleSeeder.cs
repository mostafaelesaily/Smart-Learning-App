using Microsoft.AspNetCore.Identity;

namespace Smart_Learning_App.Data.Seed
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Instructor" };

            foreach (var role in roles)
            {
                if (! await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

        }
    }
}
