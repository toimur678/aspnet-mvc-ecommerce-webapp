using EduMart.Data.Services;
using EduMart.Data;
using Microsoft.EntityFrameworkCore;
using EduMart.Data.Cart;
using Microsoft.AspNetCore.Identity;
using EduMart.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Set it with: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"")));

// Register Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Register the Service
builder.Services.AddScoped<IMentorsService, MentorsService>();
builder.Services.AddScoped<ISkillOrganizersService, SkillOrganizersService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<ISkillsService, SkillsService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc));

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Seed Users and Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AppDbInitializer.SeedUsersAndRolesAsync(app);
    // Catalogue and sample orders; each table is only seeded when empty
    AppDbInitializer.Seed(app);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Skills}/{action=Index}/{id?}");

app.Run();
