namespace ABC.Web.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unexpected error occurred");
                //await HandleExceptionAsync(context, ex);
                if (context.Response.HasStarted) throw;
                context.Response.Clear();
                context.Response.Redirect("/Home/Error", false);
            }
        }

        //private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        //{
        //    context.Response.ContentType = "application/json";
        //    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        //    var errorResponse = new
        //    {
        //        Message = "An error occurred while processing your request.",
        //        Details = exception // Optionally include more details in development
        //    };

        //    return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
        //}
    }
}
