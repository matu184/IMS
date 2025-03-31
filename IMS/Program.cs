using IMS.Data;
using IMS.Repositories.Interfaces;
using IMS.Repositories;
using IMS.Services.Interfaces;
using IMS.Services;
using Microsoft.EntityFrameworkCore;
using IMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=ims.db"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "IMS", Version = "v1" });

    // Swagger Auth einrichten
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Gib hier deinen JWT-Token ein (ohne 'Bearer' davor)."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

// Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200") 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminUserName = "admin";
    string adminPassword = "Admin123!";
    string adminRole = "Admin";

    // Rolle anlegen, falls noch nicht vorhanden
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Admin-User anlegen, falls noch nicht vorhanden
    var existingAdmin = await userManager.FindByNameAsync(adminUserName);
    if (existingAdmin == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminUserName,
            FirstName = "System",
            LastName = "Administrator",
            Email = "admin@example.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("Admin-User wurde erfolgreich angelegt.");
        }
        else
        {
            Console.WriteLine("Fehler beim Anlegen des Admin-Users:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("Admin-User existiert bereits.");
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    // Kategorien seeden
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "Hardware" },
            new Category { Name = "Zubehör" }
        );
        await context.SaveChangesAsync(); // <- wichtig!
    }

    // Lagerorte seeden
    if (!context.Locations.Any())
    {
        context.Locations.AddRange(
            new Location { Name = "Lager A" },
            new Location { Name = "Lager B" }
        );
        await context.SaveChangesAsync(); // <- wichtig!
    }

    // Produkte seeden (wenn Kategorien & Lager vorhanden sind)
    if (!context.Products.Any())
    {
        var categoryHardware = context.Categories.FirstOrDefault(c => c.Name == "Hardware");
        var categoryZubehoer = context.Categories.FirstOrDefault(c => c.Name == "Zubehör");
        var locationA = context.Locations.FirstOrDefault(l => l.Name == "Lager A");
        var locationB = context.Locations.FirstOrDefault(l => l.Name == "Lager B");

        if (categoryHardware != null && categoryZubehoer != null && locationA != null && locationB != null)
        {
            context.Products.AddRange(
                new Product
                {
                    Name = "Laptop",
                    Description = "Lenovo ThinkPad",
                    Quantity = 5,
                    MinimumQuantity = 3,
                    CategoryId = categoryHardware.Id,
                    LocationId = locationA.Id
                },
                new Product
                {
                    Name = "Maus",
                    Description = "Kabellose USB-Maus",
                    Quantity = 2,
                    MinimumQuantity = 5,
                    CategoryId = categoryZubehoer.Id,
                    LocationId = locationA.Id
                },
                new Product
                {
                    Name = "Monitor",
                    Description = "24 Zoll Full HD",
                    Quantity = 1,
                    MinimumQuantity = 2,
                    CategoryId = categoryHardware.Id,
                    LocationId = locationB.Id
                },
                new Product
                {
                    Name = "USB-Stick",
                    Description = "64GB USB 3.0",
                    Quantity = 10,
                    MinimumQuantity = 5,
                    CategoryId = categoryZubehoer.Id,
                    LocationId = locationB.Id
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Seed: Produkte erfolgreich angelegt.");
        }
        else
        {
            Console.WriteLine("❌ Seed: Kategorien oder Lagerorte fehlen – Produkte wurden nicht angelegt.");
        }
    }
}


app.Run();
