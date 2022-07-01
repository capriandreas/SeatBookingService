using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SeatBookingService.Models;
using System;
using System.Net;
using SeatBookingService.Utility;

namespace SeatBookingService.Middleware
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        string datetimenow = DateTime.Now.ToString("dd MMMM yyyy HH:mm"); 
                        string username = context.User.FindFirst("username")?.Value;
                        string path = context.Request.Path.Value;
                        string method = context.Request.Method.ToString();
                        int status_code = context.Response.StatusCode;
                        string message = contextFeature.Error.Message.ToString();
                        string stack_trace = contextFeature.Error.StackTrace.ToString();

                        ErrorDetails content = new ErrorDetails()
                        {
                            datetimenow = datetimenow,
                            username = username,
                            path = path,
                            method = method,
                            status_code = status_code,
                            message = message,
                            stack_trace = stack_trace
                        };

                        var response = DiscordUtil.SendMessageError(content);

                        await context.Response.WriteAsync(new ExceptionResponse
                        {
                           status_code = context.Response.StatusCode,
                           message = "Something went wrong..."
                        }.ToString());
                    }
                });
            });
        }
    }
}
