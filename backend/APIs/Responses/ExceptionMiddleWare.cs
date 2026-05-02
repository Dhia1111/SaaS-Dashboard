using Business;
using Business.Exceptions;
using Connection.Data;
using Connection.models;
using Microsoft.EntityFrameworkCore;
using System;

namespace APIs.Responses
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;


        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {


    

   
    

    await _next(context);  

               }

            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            var problem = new ApiProblemDetails
            {
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier
            };

            switch (ex)
            {
                case AuthenticationFailedException:
                    problem.Status = StatusCodes.Status401Unauthorized;
                    problem.Title = "Authentication failed";
                    problem.Type = "https://api.yourapp.com/errors/authentication";
                    problem.Detail = ex.Message;
                    break;

                case ResourceAlreadyExistsException:
                    problem.Status = StatusCodes.Status409Conflict;
                    problem.Title = "Resource already exists";
                    problem.Type = "https://api.yourapp.com/errors/conflict";
                    problem.Detail = ex.Message;
                    break;

                case ArgumentException:
                    problem.Status = StatusCodes.Status400BadRequest;
                    problem.Title = "Invalid request";
                    problem.Type = "https://api.yourapp.com/errors/validation";
                    problem.Detail = ex.Message;
                    break;

                default:
                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Title = "Internal server error";
                    problem.Type = "https://api.yourapp.com/errors/internal";
                    problem.Detail = ex.ToString();
                    break;
            }

            _logger.LogError(ex, "Exception occurred: {TraceId}", problem.TraceId);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = problem.Status.Value;

            return context.Response.WriteAsJsonAsync(problem);
        }
    }
}
