using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuizSystem.Common.Common;

namespace QuizSystem.DAL.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");

            var rolesToCheck = new[] { AppRoles.Instructor, AppRoles.Student };

            foreach (var roleName in rolesToCheck)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                    if (result.Succeeded)
                    {
                        logger.LogInformation("Role '{RoleName}' created successfully.", roleName);
                    }
                    else
                    {
                        var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogError("Error creating role '{RoleName}': {Error}", roleName, errorMsg);
                    }
                }
            }
        }
    }
}