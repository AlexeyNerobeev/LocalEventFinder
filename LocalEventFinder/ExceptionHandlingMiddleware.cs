using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace LocalEventFinder
{
    /// <summary>
    /// Глобальный обработчик исключений
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        /// <summary>
        /// Конструктор middleware обработки исключений
        /// </summary>
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Обработка запроса
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошло необработанное исключение");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Обработка исключения и формирование ответа
        /// </summary>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                SecurityTokenException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                error = new
                {
                    message = statusCode switch
                    {
                        StatusCodes.Status401Unauthorized => "Неавторизованный доступ",
                        StatusCodes.Status403Forbidden => "Доступ запрещен",
                        _ => "Внутренняя ошибка сервера"
                    },
                    type = exception.GetType().Name,
                    details = _env.IsDevelopment() ? exception.Message : GetUserFriendlyMessage(exception),
                    stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
                },
                timestamp = DateTime.UtcNow
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment()
            };

            var json = JsonSerializer.Serialize(response, jsonOptions);
            return context.Response.WriteAsync(json);
        }

        private string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException => "Неверные учетные данные или истекший токен",
                SecurityTokenException => "Недействительный токен",
                _ => "Обратитесь в службу поддержки"
            };
        }
    }

    /// <summary>
    /// Extension methods для регистрации middleware
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Регистрация middleware обработки исключений
        /// </summary>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
