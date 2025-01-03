using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenantIdentityDemo.Data;
using MultiTenantIdentityDemo.Manager;
using MultiTenantIdentityDemo.Models;
using Finbuckle.MultiTenant.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Default connection string for the main identity database
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure Multi-Tenant with EFCoreStore
builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithEFCoreStore<TenantDbContext, AppTenantInfo>()
    .WithHostStrategy(); // Identify tenants by hostnames

// Add TenantManager for dynamic tenant operations
builder.Services.AddScoped<TenantManager>();

// Configure Tenant-specific DbContext
// Configure Tenant-specific DbContext
builder.Services.AddDbContext<TenantDbContext>((sp, options) =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var context = httpContextAccessor.HttpContext;
    var tenantInfo = context?.Items["TenantContext"] as AppTenantInfo;

    if (tenantInfo != null && !string.IsNullOrEmpty(tenantInfo.ConnectionString))
    {
        options.UseSqlServer(tenantInfo.ConnectionString); // Tenant veritabanını kullan
    }
    else
    {
        // Varsayılan bağlantı dizesini kullanarak TenantDbContext'i oluştur
        options.UseSqlServer(defaultConnection);
        Console.WriteLine("Tenant context not found. Using default connection.");
    }
});




// Configure Identity (DefaultDb)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(defaultConnection);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Ensure DefaultDb is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}
 
// TenantDb oluştur ve kontrol et
using (var scope = app.Services.CreateScope())
{
    var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<TenantDbContext>>();
    using var tenantDbContext = new TenantDbContext(options);
    await tenantDbContext.Database.EnsureCreatedAsync();
}

// Middleware configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMultiTenant();
app.UseAuthentication();
app.UseAuthorization();

// Middleware to load tenant info based on user email
app.Use(async (context, next) =>
{
    var tenantStore = context.RequestServices.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
    var userEmail = context.User?.Identity?.Name;

    if (!string.IsNullOrEmpty(userEmail))
    {
        var tenantId = userEmail.Split('@')[0];
        var tenantInfo = await tenantStore.TryGetAsync(tenantId);

        if (tenantInfo != null)
        {
            context.Items["TenantContext"] = tenantInfo; // Sadece TenantInfo saklanıyor
        }
    }

    await next();
});


// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
