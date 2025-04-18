using CinemaDomain.Model;
using Microsoft.AspNetCore.Identity;

namespace CinemaInfrastructure.ViewModel
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string superAdminEmail = "admin@gmail.com";
            string superAdminPassword = "Qwerty_1";

            // Створення ролі superadmin, якщо її ще немає
            if (await roleManager.FindByNameAsync("superadmin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("superadmin"));
            }

            // Створення ролі user, якщо її ще немає
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            // Створення супер-адміна, якщо його ще немає
            if (await userManager.FindByEmailAsync(superAdminEmail) == null)
            {
                User admin = new User { Email = superAdminEmail, UserName = superAdminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, superAdminPassword);
                if (result.Succeeded)
                {
                    // Призначення ролі superadmin
                    await userManager.AddToRoleAsync(admin, "superadmin");
                }
            }
        }
    }
}
