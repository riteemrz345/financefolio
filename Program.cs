using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using FinanceFolio.Data;
using FinanceFolio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddIdentity<MongoIdentityUser, MongoIdentityRole>()
.AddMongoDbStores<MongoIdentityUser,MongoIdentityRole, Guid>
(builder.Configuration.GetSection("MongoDB:ConnectionString").Value, builder.Configuration.GetSection("MongoDB:DatabaseName").Value);



builder.Services.AddDbContext<AppDbcontext>(options => options.UseMongoDB(builder.Configuration.GetSection("MongoDB:ConnectionString").Value, builder.Configuration.GetSection("MongoDB:DatabaseName").Value));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "FinanceFolio.Cookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Account/Login"; // Customize as needed
    options.AccessDeniedPath = "/Account/AccessDenied"; // Customize as needed
    options.SlidingExpiration = true;
});

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceFolio", Version = "v1" });

    // Define the cookie authentication scheme
    c.AddSecurityDefinition("CookieAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "FinanceFolio.Cookie", // The name of the cookie
        In = ParameterLocation.Cookie,
        Description = "Identity cookie authentication"
    });

    // Make sure Swagger UI requires the cookie
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "cookieAuth"
                    }
                },
                new List<string>()
            }
        });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetValue<string>("Cors", "*").Split(',', System.StringSplitOptions.RemoveEmptyEntries))
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
