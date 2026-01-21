using Booking.Application.Profile;
using Booking.Application.Services;
using Booking.DataAccess.Context;
using Booking.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BookingDbContext>()
    .AddDefaultTokenProviders();

// Configure PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Configure DbContext with proper migration assembly
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly("Booking.DataAccess");
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        }));

// Register repositories (update namespaces as needed)
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Register application services
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Add AutoMapper
builder.Services.AddAutoMapper(
    cfg => {
        cfg.AddProfile<RoomProfile>();
        cfg.AddProfile<BookingProfile>();
    },
    typeof(Program).Assembly
);

// Add logging
builder.Services.AddLogging();

// Add authentication
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Booking API",
        Version = "v1",
        Description = "API for room booking management"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations and seed data
await ApplyMigrationsAndSeedData(app);

app.Run();

async Task ApplyMigrationsAndSeedData(WebApplication webApp)
{
    using var scope = webApp.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    try
    {
        var context = services.GetRequiredService<BookingDbContext>();
        
        logger.LogInformation("Checking for pending migrations...");
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Get pending migrations
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var migrationsList = pendingMigrations.ToList();
        
        if (migrationsList.Any())
        {
            logger.LogInformation("Found {Count} pending migration(s): {Migrations}",
                migrationsList.Count, string.Join(", ", migrationsList));
            
            // Apply migrations
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("No pending migrations found.");
        }

        // Check if tables exist
        var roomTableExists = await context.Database.CanConnectAsync() && 
                            context.Database.GetDbConnection().State == System.Data.ConnectionState.Open;
        
        if (!roomTableExists)
        {
            logger.LogWarning("Database connection issue after migrations.");
        }

        // Seed roles
        await SeedRolesAsync(services, logger);

        // Seed default admin if configured
        await SeedDefaultAdminAsync(configuration, services, logger);

        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
        
        // For development, you might want to rethrow
        if (webApp.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

async Task SeedRolesAsync(IServiceProvider services, ILogger logger)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            logger.LogInformation("Created role: {Role}", role);
        }
        else
        {
            logger.LogDebug("Role {Role} already exists", role);
        }
    }
}

async Task SeedDefaultAdminAsync(IConfiguration configuration, IServiceProvider services, ILogger logger)
{
    var adminSettings = configuration.GetSection("AdminSettings");
    var adminEmail = adminSettings["Email"];
    var adminPassword = adminSettings["Password"];

    if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
    {
        logger.LogInformation("Admin credentials not configured. Skipping admin user creation.");
        return;
    }

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // Check if admin user already exists
    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin != null)
    {
        logger.LogInformation("Admin user already exists.");

        // Ensure admin has Admin role
        if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
        {
            await userManager.AddToRoleAsync(existingAdmin, "Admin");
            logger.LogInformation("Added Admin role to existing user.");
        }

        return;
    }

    // Create admin user
    var adminUser = new IdentityUser
    {
        UserName = adminEmail,
        Email = adminEmail,
        EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(adminUser, adminPassword);

    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
        logger.LogInformation("Default admin user created with email: {Email}", adminEmail);
    }
    else
    {
        logger.LogError("Failed to create admin user: {Errors}",
            string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
