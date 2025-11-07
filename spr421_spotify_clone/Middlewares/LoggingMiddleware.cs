namespace spr421_spotify_clone.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Request
            var startDate = DateTime.UtcNow;
            var req = context.Request;

            _logger.LogWarning($"{req.Protocol} " +
                $"\n {req.Path} " +
                $"\n {req.Host} " +
                $"\n {req.QueryString} " +
                $"\n {req.Method} " +
                $"\n {req.IsHttps} " +
                $"\n {req.ContentLength} " +
                $"\n {req.ContentType}");

            await _next(context);

            // Response
            var res = context.Response;
            var headers = res.Headers;
            string logMessage = $"\nStatusCode: {res.StatusCode} " +
                $"\nContentType: {res.ContentType} " +
                $"\nContentLength: {res.ContentLength} ";

            foreach(var header in headers)
            {
                logMessage += $"\nHeader: {header.Key} - Value: {header.Value}";
            }

            var endDate = DateTime.UtcNow;
            var duration = (endDate - startDate).TotalMilliseconds;

            logMessage += $"\nDuration: {duration} ms";

            _logger.LogCritical(logMessage);
        }
    }
}
