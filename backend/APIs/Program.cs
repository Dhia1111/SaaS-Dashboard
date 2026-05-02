using APIs.AssetHandler;
using APIs.BackGroundJobs;
using APIs.ConfigClasses;
using APIs.Hashing;
using APIs.Responses;
using Business;
using Connection;
using Connection.Data;
using Connection.models;
using ExternalAPI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration["DefaultConnection"];
builder.Services.Configure<ExternalAPI.EmailSettings>(
  builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<Business.EmailSettings>(
  builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<Business.JwtSetting>(
  builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<ClientInfo>(
    builder.Configuration.GetSection("ClientInfo"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
   options.Cookie.Name = "ExternalAuthCookie";
   options.Cookie.SameSite = SameSiteMode.Lax; // Required for cross-port redirects
})
.AddGoogle(options =>
{
   options.ClientId = builder.Configuration["Google:ClientID"];
   options.ClientSecret = builder.Configuration["Google:ClientSecret"];
 
    // --- THIS IS THE FIX ---
    // The correlation cookie is what validates the "state"
   options.CorrelationCookie.HttpOnly = true;
   options.CorrelationCookie.SameSite = SameSiteMode.Lax;
   options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    // ------------------------
});
if (connString == null)
{
    throw new Exception("Connection string does not exist");
}

builder.Services.AddDbContextFactory<SaasDashboardContext>(options =>
{
    options.UseNpgsql(connString);
    
});
builder.Services.AddControllers();
builder.Services.AddAPIDependencies();
builder.Services.AddConnectionDependencies();
builder.Services.AddExternalAPIDependencies();
builder.Services.AddBusinessDependencies();
// Register your job as a scoped service (required for DbContext)
builder.Services.AddScoped<EmailBackgroundJob>();
// Add Quartz services
builder.Services.AddQuartz(q =>
{
    // Use persistent store with PgServer
    q.UsePersistentStore(options =>
    {
        // Configure PgServer connection
        options.UsePostgres(PgServer =>
        {

            PgServer.ConnectionString =connString;
            PgServer.TablePrefix = "qrtz_";



        });
       // options.Properties.Set("quartz.jobStore.databaseUpdate", "true"); // For Quartz 3.x



        // IMPORTANT: UseProperties = true allows storing job data
        options.UseProperties = true;

        // Use JSON serialization (recommended over binary)
        options.UseNewtonsoftJsonSerializer();

        // Configure retry behavior
        options.RetryInterval = TimeSpan.FromSeconds(30);
        options.PerformSchemaValidation = true;

        // Configure clustering if needed (for high availability)
        // options.UseClustering(c =>
        // {
        //     c.CheckinInterval = TimeSpan.FromSeconds(10);
        //     c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
        // });

    });


    // Use dedicated thread pool (replaces SimpleThreadPool)
    q.UseDedicatedThreadPool(tp =>
    {
        tp.MaxConcurrency = 10;  // Controls max concurrent jobs
    });

    // Register durable job
    var emailJobKey = new JobKey("SendEmailJob", "EmailGroup");

    q.AddJob<EmailBackgroundJob>(opts => opts
        .WithIdentity(emailJobKey)
        .StoreDurably()  // Job survives even with no triggers
            .UsingJobData("ForceSend", "false")  // Add this line - set default value

    );
    //   TRIGGER EVERY 1 MINUTE using cron expression
    q.AddTrigger(opts => opts
        .ForJob(emailJobKey)
        .WithIdentity("EmailTrigger", "EmailProcessing")
        .WithCronSchedule("0 */1 * ? * *")  // Every 1 minute
        .WithDescription("Triggers email processing every 1 minute")
    );
});
// Add Quartz hosted service (starts scheduler with the app)
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;  // Graceful shutdown
    options.AwaitApplicationStarted = true; // Start after app starts
});  
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
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ApiProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Type = "https://api.yourapp.com/errors/validation",
            Instance = context.HttpContext.Request.Path,
            TraceId = context.HttpContext.TraceIdentifier,
            Errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    k => k.Key,
                    v => v.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
        };

        return new BadRequestObjectResult(problem);
    };
});
var app = builder.Build();

// 1. Exception handling must be FIRST to catch errors in all subsequent steps
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. HTTPS enforcement should happen BEFORE Auth
app.UseHttpsRedirection();

// 3. CORS must be before Auth
app.UseCors("AllowReactApp");

// 4. ROUTING must exist for Auth to map the callback
app.UseRouting();

// 5. AUTHENTICATION & AUTHORIZATION
app.UseAuthentication();
app.UseAuthorization();

// 6. ENDPOINTS
app.MapControllers();

app.Run();