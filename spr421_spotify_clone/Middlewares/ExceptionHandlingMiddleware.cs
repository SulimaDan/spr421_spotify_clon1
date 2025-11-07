using spr421_spotify_clone.BLL.Services;

namespace spr421_spotify_clone.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

                var responseMessage = ex.Message;
                if(ex.InnerException != null)
                {
                    responseMessage += $". Inner exception message: {ex.InnerException.Message}";
                }

                var response = new ServiceResponse
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = responseMessage
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
