using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using FinanceFolio.Data;
using FinanceFolio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.LoginPath = "/Account/Login";
    options.SlidingExpiration = true;
});
builder.Services.AddDbContext<AppDbcontext>(options => options.UseMongoDB(builder.Configuration.GetSection("MongoDB:ConnectionString").Value, builder.Configuration.GetSection("MongoDB:DatabaseName").Value));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
