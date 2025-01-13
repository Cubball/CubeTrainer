using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorization();
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();