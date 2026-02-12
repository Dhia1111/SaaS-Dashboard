using Business;
using Connection;
using Connection.Data;
using ExternalAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration["DefaultConnection"];
if (connString == null)
{
    throw new Exception("Connection string does not exist");
}
builder.Services.AddDbContextPool<SaasDashboardContext>(options =>
{
    options.UseNpgsql(connString);
});
builder.Services.AddControllers();

builder.Services.AddBusinessDependencies();
builder.Services.AddConnectionDependencies();
builder.Services.AddExternalAPIDependencies();


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
