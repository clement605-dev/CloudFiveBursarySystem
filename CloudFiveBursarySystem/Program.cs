using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// 1. DATABASE CONNECTION
// --------------------------------------------------

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// --------------------------------------------------
// 2. IDENTITY AND USER AUTHENTICATION
// --------------------------------------------------

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Allow applicants to register without email confirmation.
    options.SignIn.RequireConfirmedAccount = false;

    // Password security requirements.
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


// --------------------------------------------------
// 3. APPLICATION SERVICES
// --------------------------------------------------

// Registers the notification service.
// Other controllers can use it to create and retrieve notifications.
builder.Services.AddScoped<NotificationService>();

// Allows controllers and services to access the current HTTP user.
builder.Services.AddHttpContextAccessor();

// Registers the administrator activity logging service.
builder.Services.AddScoped<ActivityLogService>();

// --------------------------------------------------
// 4. MVC AND RAZOR PAGES
// --------------------------------------------------

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


// --------------------------------------------------
// 5. BUILD THE APPLICATION
// --------------------------------------------------

var app = builder.Build();


// --------------------------------------------------
// 6. CREATE DEFAULT USER ROLES
// --------------------------------------------------

// Ensure the Applicant and Administrator roles exist.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles =
    {
        "Applicant",
        "Administrator"
    };

    foreach (var role in roles)
    {
        bool roleExists = await roleManager.RoleExistsAsync(role);

        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


// --------------------------------------------------
// 7. APPLICATION ERROR HANDLING
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// --------------------------------------------------
// 8. HTTP SECURITY AND REQUEST PIPELINE
// --------------------------------------------------

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// Authentication identifies the logged-in user.
app.UseAuthentication();

// Authorization checks the user's permissions and roles.
app.UseAuthorization();


// --------------------------------------------------
// 9. APPLICATION ROUTING
// --------------------------------------------------

// Register controllers that use attribute-based routes.
app.MapControllers();

// Register the default conventional MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Maps ASP.NET Core Identity pages.
app.MapRazorPages();


// --------------------------------------------------
// 10. START THE APPLICATION
// --------------------------------------------------

// TEMPORARY TEST: Check whether the application registers this endpoint.
app.MapGet("/activity-route-test", () =>
    "Activity route test is working.");

app.MapGet("/debug-endpoints", (
    IEnumerable<Microsoft.AspNetCore.Routing.EndpointDataSource> dataSources) =>
{
    var endpoints = dataSources
        .SelectMany(source => source.Endpoints)
        .Select(endpoint => endpoint.DisplayName);

    return string.Join("\n", endpoints);
});
app.Run();
