using Business;
using Connection;
using Connection.Data;
using ExternalAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? st = builder.Configuration["ConnectionSetting:DefaultConnection"];
// Add services to the container.
builder.Services.AddBusinessDependencies();
builder.Services.AddConnectionDependencies();
builder.Services.AddExternalAPIDependencies();

builder.Services.AddDbContextPool<SaasDashboardContext>(options =>

options.UseNpgsql(
    st
));


builder.Services.AddControllers();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();

    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();
