using N5.Employees.Application.Exceptions;
using System;
using System.Net;
using System.Text.Json;

namespace N5.Employees.Api.Middlewares
{
	public class ExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlerMiddleware> _logger;

		public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception while processing request {0} {1}", context.Request.Method, context.Request.Path);
				_logger.LogError("Exception Type: {0}", ex.GetType().Name);
				_logger.LogError("RequestId: {0}", context.TraceIdentifier);

				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var statusCode = HttpStatusCode.InternalServerError;
			var message = "An unexpected error occurred";

			switch (exception)
			{
				case NotFoundException:
					statusCode = HttpStatusCode.NotFound;
					message = exception.Message;
					break;

				case BadRequestException:
					statusCode = HttpStatusCode.BadRequest;
					message = exception.Message;
					break;
			}

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)statusCode;

			var result = JsonSerializer.Serialize(new { error = message });

			return context.Response.WriteAsync(result);
		}
	}
}