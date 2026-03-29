using APIs.AssetHandler;
using APIs.Hashing;
using APIs.Responses;
using Business;
using Connection;
using Connection.Data;
using ExternalAPI;
using Microsoft.AspNetCore.Diagnostics;
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
builder.Services.AddAPIDependencies();
builder.Services.AddConnectionDependencies();
builder.Services.AddExternalAPIDependencies();
builder.Services.AddBusinessDependencies();


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
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var logger = context.RequestServices
                .GetRequiredService<ILogger<Program>>();

            var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionFeature?.Error;

            if (exception != null)
            {
                logger.LogCritical(exception, "Unhandled exception occurred");
            }

            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(
                ApiResult<object>.Fail(
                    "SERVER_ERROR",
                    "Unexpected server error"
                ));
        });
    });

}
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowReactApp");
app.MapControllers();
app.Run();
