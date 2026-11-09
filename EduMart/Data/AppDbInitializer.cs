using EduMart.Data.Enums;
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

        /// <summary>
        /// Seeds the course catalogue (platforms, agencies, mentors, skills) and sample orders.
        /// Each block only runs when its table is empty, so it never overwrites edited data.
        /// Run after <see cref="SeedUsersAndRolesAsync"/> so the sample orders can find their users.
        /// Course dates are relative to the day the seed runs, so a fresh database always
        /// shows a mix of available, upcoming and expired courses.
        /// </summary>
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var today = DateTime.Today;

            // Platforms (where a course is hosted)
            if (!context.Platforms.Any())
            {
                context.Platforms.AddRange(
                    new Platforms { PlatformName = "Udemy", PlatformLogo = "/images/platforms/udemy.svg", Description = "Self-paced video courses you buy once and keep, with lifetime access to updates." },
                    new Platforms { PlatformName = "Coursera", PlatformLogo = "/images/platforms/coursera.svg", Description = "Structured courses with graded assignments and a certificate on completion." },
                    new Platforms { PlatformName = "edX", PlatformLogo = "/images/platforms/edx.svg", Description = "University-style courses with weekly modules, readings and quizzes." },
                    new Platforms { PlatformName = "Skillshare", PlatformLogo = "/images/platforms/skillshare.svg", Description = "Short project-based classes, each ending with something you make and share." },
                    new Platforms { PlatformName = "Zoom", PlatformLogo = "/images/platforms/zoom.svg", Description = "Live classes held on Zoom video calls, with the recording sent afterwards." }
                );
                context.SaveChanges();
            }

            // Skill organizers (shown as "Agencies" in the app)
            if (!context.Skill_Organizers.Any())
            {
                context.Skill_Organizers.AddRange(
                    new SkillOrganizers { FullName = "Harbourside Learning Co-op", ProfilePictureURL = "/images/agencies/harbourside.svg", About = "A member-run co-op that books teachers for evening and weekend technology courses." },
                    new SkillOrganizers { FullName = "Northfield Kitchen School", ProfilePictureURL = "/images/agencies/northfield.svg", About = "A cookery school that runs small hands-on classes, live and recorded." },
                    new SkillOrganizers { FullName = "Ledger & Loom Studio", ProfilePictureURL = "/images/agencies/ledger-and-loom.svg", About = "Money and business courses for freelancers, makers and small studios." },
                    new SkillOrganizers { FullName = "Common Ground Fitness", ProfilePictureURL = "/images/agencies/common-ground.svg", About = "Community fitness classes for people starting from zero." },
                    new SkillOrganizers { FullName = "Lingua Hall", ProfilePictureURL = "/images/agencies/lingua-hall.svg", About = "A language school teaching conversation first, grammar second." }
                );
                context.SaveChanges();
            }

            // Mentors
            if (!context.Mentors.Any())
            {
                context.Mentors.AddRange(
                    new Mentors { FullName = "Amara Okafor", ProfilePictureURL = "/images/mentors/amara-okafor.svg", About = "Data analyst who teaches Python to people who live in spreadsheets." },
                    new Mentors { FullName = "Daniel Kowalski", ProfilePictureURL = "/images/mentors/daniel-kowalski.svg", About = "Electronics technician who builds and repairs small hardware projects." },
                    new Mentors { FullName = "Hannah Becker", ProfilePictureURL = "/images/mentors/hannah-becker.svg", About = "Baker who teaches long-fermentation bread at home-oven scale." },
                    new Mentors { FullName = "Tomás Herrera", ProfilePictureURL = "/images/mentors/tomas-herrera.svg", About = "Line cook turned teacher, focused on knife work and kitchen prep." },
                    new Mentors { FullName = "Mei Lin Zhang", ProfilePictureURL = "/images/mentors/mei-lin-zhang.svg", About = "Noodle maker who teaches hand-pulled and knife-cut noodles." },
                    new Mentors { FullName = "Priya Raman", ProfilePictureURL = "/images/mentors/priya-raman.svg", About = "Accountant for freelancers and sole traders." },
                    new Mentors { FullName = "Jonas Lindqvist", ProfilePictureURL = "/images/mentors/jonas-lindqvist.svg", About = "Workshop facilitator for remote and hybrid teams." },
                    new Mentors { FullName = "Grace Mensah", ProfilePictureURL = "/images/mentors/grace-mensah.svg", About = "Yoga teacher who runs beginner and mobility classes." },
                    new Mentors { FullName = "Sam Whitfield", ProfilePictureURL = "/images/mentors/sam-whitfield.svg", About = "Running coach and strength trainer for adult beginners." },
                    new Mentors { FullName = "Ji-woo Park", ProfilePictureURL = "/images/mentors/ji-woo-park.svg", About = "Korean teacher who builds lessons around travel situations." },
                    new Mentors { FullName = "Lucas Moreau", ProfilePictureURL = "/images/mentors/lucas-moreau.svg", About = "French teacher who runs small conversation groups." },
                    new Mentors { FullName = "Yuki Tanaka", ProfilePictureURL = "/images/mentors/yuki-tanaka.svg", About = "Japanese teacher who works with spaced-repetition decks." }
                );
                context.SaveChanges();
            }

            // Skills (courses) and their mentors
            if (!context.Skills.Any())
            {
                var platform = context.Platforms.ToDictionary(p => p.PlatformName, p => p.Id);
                var agency = context.Skill_Organizers.ToDictionary(o => o.FullName, o => o.Id);
                var mentor = context.Mentors.ToDictionary(m => m.FullName, m => m.Id);

                var courses = new[]
                {
                    // name, description, price, image, category, platform, agency, start, end, mentors
                    ("Python for Spreadsheet Users", "Move your weekly reports from spreadsheets to Python and pandas, one real file at a time.", 89.0, "python-data", Skill_Category.Technology, "Udemy", "Harbourside Learning Co-op", -14, 30, new[] { "Amara Okafor" }),
                    ("A Home Server on a Raspberry Pi", "Set up a Raspberry Pi as a file, backup and media server for your home network.", 59.0, "raspberry-pi", Skill_Category.Technology, "edX", "Harbourside Learning Co-op", 12, 40, new[] { "Daniel Kowalski" }),
                    ("Soldering and Small Electronics", "Solder through-hole and surface-mount parts, then repair a broken cable and a loose jack.", 45.0, "soldering", Skill_Category.Technology, "Zoom", "Harbourside Learning Co-op", -3, 18, new[] { "Daniel Kowalski" }),
                    ("Sourdough from Scratch", "Keep a starter alive, mix a dough and bake an open-crumb loaf in a home oven.", 39.0, "sourdough", Skill_Category.Cooking, "Skillshare", "Northfield Kitchen School", -20, 10, new[] { "Hannah Becker" }),
                    ("Knife Skills for Home Cooks", "Hold, sharpen and use a chef's knife for faster, safer prep.", 29.0, "knife-skills", Skill_Category.Cooking, "Zoom", "Northfield Kitchen School", 5, 12, new[] { "Tomás Herrera" }),
                    ("Hand-pulled Noodles", "Mix, rest and pull wheat noodles by hand, with two sauces to serve them.", 49.0, "noodles", Skill_Category.Cooking, "Zoom", "Northfield Kitchen School", -40, -5, new[] { "Mei Lin Zhang", "Tomás Herrera" }),
                    ("Bookkeeping for Freelancers", "Track income, expenses and tax set-asides in one simple ledger.", 69.0, "bookkeeping", Skill_Category.Business, "Coursera", "Ledger & Loom Studio", -7, 45, new[] { "Priya Raman" }),
                    ("Pricing Handmade Work", "Work out what your hours and materials cost, then set prices that cover them.", 55.0, "pricing-handmade", Skill_Category.Business, "Skillshare", "Ledger & Loom Studio", -2, 26, new[] { "Priya Raman", "Jonas Lindqvist" }),
                    ("Running Remote Workshops", "Plan and facilitate a two-hour online workshop that ends with decisions, not more meetings.", 120.0, "remote-workshops", Skill_Category.Business, "Coursera", "Ledger & Loom Studio", 20, 50, new[] { "Jonas Lindqvist" }),
                    ("Yoga for Stiff Beginners", "Six weeks of short sessions for mobility, balance and breathing.", 35.0, "yoga", Skill_Category.Health, "Zoom", "Common Ground Fitness", -10, 32, new[] { "Grace Mensah" }),
                    ("Couch to 5K", "A nine-week run-walk plan that gets you running 5 km without stopping.", 25.0, "couch-to-5k", Skill_Category.Health, "Udemy", "Common Ground Fitness", -30, 33, new[] { "Sam Whitfield" }),
                    ("Strength Training Basics", "Squat, hinge, push and pull with good form, using dumbbells at home or in a gym.", 45.0, "strength", Skill_Category.Health, "Udemy", "Common Ground Fitness", -60, -14, new[] { "Sam Whitfield", "Grace Mensah" }),
                    ("Korean for Travellers", "Read Hangul and handle cafés, transport and directions in Seoul.", 79.0, "korean", Skill_Category.Languages, "edX", "Lingua Hall", -5, 55, new[] { "Ji-woo Park" }),
                    ("Conversational French", "Weekly speaking practice in small groups for confident A2 learners.", 95.0, "french", Skill_Category.Languages, "Zoom", "Lingua Hall", 3, 60, new[] { "Lucas Moreau" }),
                    ("Japanese: Your First 500 Words", "Hiragana, katakana and the 500 words you will use most, drilled with spaced repetition.", 85.0, "japanese", Skill_Category.Languages, "edX", "Lingua Hall", -12, 48, new[] { "Yuki Tanaka" }),
                };

                foreach (var c in courses)
                {
                    var skill = new Skills
                    {
                        SkillName = c.Item1,
                        SkillDescription = c.Item2,
                        Price = c.Item3,
                        ImageURL = $"/images/skills/{c.Item4}.jpg",
                        SkillCategory = c.Item5,
                        PlatformId = platform[c.Item6],
                        SkillOrganizerID = agency[c.Item7],
                        StartDate = today.AddDays(c.Item8),
                        EndDate = today.AddDays(c.Item9),
                    };
                    context.Skills.Add(skill);
                    context.SaveChanges();

                    foreach (var name in c.Item10)
                    {
                        context.Mentors_Skills.Add(new Mentors_Skills { SkillID = skill.Id, MentorID = mentor[name] });
                    }
                    context.SaveChanges();
                }
            }

            // Sample orders for the sample users
            if (!context.Orders.Any())
            {
                var skillsByName = context.Skills.ToDictionary(s => s.SkillName, s => s);
                var usersByEmail = context.Users.ToDictionary(u => u.Email!, u => u);

                var orders = new (string Email, (string Skill, int Amount)[] Items)[]
                {
                    ("leila.haddad@example.com", new[] { ("Python for Spreadsheet Users", 1), ("Bookkeeping for Freelancers", 1) }),
                    ("marco.bianchi@example.com", new[] { ("Sourdough from Scratch", 2) }),
                    ("aisha.bello@example.com", new[] { ("Yoga for Stiff Beginners", 1), ("Couch to 5K", 1) }),
                    ("ethan.clarke@example.com", new[] { ("A Home Server on a Raspberry Pi", 1), ("Soldering and Small Electronics", 1) }),
                    ("sofia.petrova@example.com", new[] { ("Conversational French", 1) }),
                    ("kenji.watanabe@example.com", new[] { ("Japanese: Your First 500 Words", 1), ("Knife Skills for Home Cooks", 1) }),
                    ("nadia.rahman@example.com", new[] { ("Pricing Handmade Work", 1), ("Bookkeeping for Freelancers", 1) }),
                    ("oliver.grant@example.com", new[] { ("Strength Training Basics", 1), ("Couch to 5K", 1) }),
                    ("leila.haddad@example.com", new[] { ("Korean for Travellers", 1) }),
                    ("marco.bianchi@example.com", new[] { ("Hand-pulled Noodles", 1) }),
                };

                foreach (var (email, items) in orders)
                {
                    if (!usersByEmail.TryGetValue(email, out var user)) continue;

                    var order = new Order { UserId = user.Id, Email = email };
                    context.Orders.Add(order);
                    context.SaveChanges();

                    foreach (var (skillName, amount) in items)
                    {
                        var skill = skillsByName[skillName];
                        context.OrderItems.Add(new OrderItem { OrderId = order.Id, SkillId = skill.Id, Amount = amount, Price = skill.Price });
                    }
                    context.SaveChanges();
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
                var config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var env = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                // The demo passwords are published in the README, so they are only used in Development.
                // Other environments must set Seed:AdminEmail and Seed:AdminPassword
                // (e.g. environment variables Seed__AdminEmail / Seed__AdminPassword) and get no sample learners.
                string? adminUserEmail = config["Seed:AdminEmail"] ?? (env.IsDevelopment() ? "admin@gmail.com" : null);
                string? adminUserPassword = config["Seed:AdminPassword"] ?? (env.IsDevelopment() ? "Admin@1234" : null);

                if (adminUserEmail == null || adminUserPassword == null)
                {
                    logger.LogWarning("No admin account seeded: set Seed:AdminEmail and Seed:AdminPassword to create one.");
                }
                else if (await userManager.FindByEmailAsync(adminUserEmail) == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = "Admin User",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    // Password must satisfy the default Identity policy (uppercase, lowercase, digit, symbol)
                    var result = await userManager.CreateAsync(newAdminUser, adminUserPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                    }
                    else
                    {
                        logger.LogError("Seeding {Email} failed: {Errors}", adminUserEmail, string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }

                // Sample learners share a published password, so they only exist in Development
                if (!env.IsDevelopment()) return;

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
