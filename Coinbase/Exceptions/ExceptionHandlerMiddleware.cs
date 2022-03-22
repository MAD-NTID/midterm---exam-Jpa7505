using System;
using System.Text.Json;
using System.Threading.Tasks;
using Coinbase.Services;
using Microsoft.AspNetCore.Http;

namespace Coinbase.Exceptions
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.Info(context.Request.Method + "- "+ context.Request.Path);
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode = 500;
            string message = "An internal error occurred";
            
            if (exception is IUserErrorException)
            {
                UserErrorException err = (UserErrorException) exception;
                statusCode = err.GetStatusCode();
                message = err.GetMessage();
            }
            
            if (statusCode == 500)
            {
                _logger.Error(exception.Message);
            }
            
            //we want the response to output as json
            context.Response.ContentType = "application/json";
            string errorString = JsonSerializer.Serialize(new {Message = message, StatusCode = statusCode});
            await context.Response.WriteAsync(errorString);
        }
    }
}