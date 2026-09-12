using EduMart.Data.Static;
using EduMart.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace EduMart.Data
{
    public class AppDbInitializer
    {
        // Sample learners seeded with the User role. All share the password below.
        // Emails use example.com so they can never reach a real inbox.
        public const string SampleUserPassword = "User@1234";

        private static readonly (string FullName, string Email)[] SampleUsers =
        {
            ("Leila Haddad", "leila.haddad@example.com"),
            ("Marco Bianchi", "marco.bianchi@example.com"),
            ("Aisha Bello", "aisha.bello@example.com"),
            ("Ethan Clarke", "ethan.clarke@example.com"),
            ("Sofia Petrova", "sofia.petrova@example.com"),
            ("Kenji Watanabe", "kenji.watanabe@example.com"),
            ("Nadia Rahman", "nadia.rahman@example.com"),
            ("Oliver Grant", "oliver.grant@example.com"),
        };

        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();

                // Platforms
                if (!context.Platforms.Any()) 
                {

                }

                // Mentor
                if (!context.Mentors.Any())
                {

                }
                // Skill_Organizer
                if (!context.Skill_Organizers.Any())
                {

                }
                // Skills
                if (!context.Skills.Any())
                {

                }
                // Mentors & Skills
                if (!context.Mentors_Skills.Any())
                {

                }
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                // Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                // Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<AppDbInitializer>>();
                string adminUserEmail = "admin@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = "Admin User",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    // Password must satisfy the default Identity policy (uppercase, lowercase, digit, symbol)
                    var result = await userManager.CreateAsync(newAdminUser, "Admin@1234");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                    }
                    else
                    {
                        logger.LogError("Seeding {Email} failed: {Errors}", adminUserEmail, string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }

                // Sample learners (UserName = email, as the Register action does)
                foreach (var (fullName, email) in SampleUsers)
                {
                    if (await userManager.FindByEmailAsync(email) != null) continue;

                    var newAppUser = new ApplicationUser()
                    {
                        FullName = fullName,
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(newAppUser, SampleUserPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                    }
                    else
                    {
                        logger.LogError("Seeding {Email} failed: {Errors}", email, string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

    }
}
