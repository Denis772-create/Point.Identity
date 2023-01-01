namespace Point.Services.Identity.Web.Infrastructure.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<HttpGlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HttpGlobalExceptionFilter(IWebHostEnvironment webHostEnvironment,
        ILogger<HttpGlobalExceptionFilter> logger)
    {
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);


        var json = new JsonErrorResponse
        {
            Messages = new[] { "An error occur. Try it again." }
        };

        if (_webHostEnvironment.IsDevelopment()) json.DeveloperMessage = context.Exception;

        context.Result = new InternalServerErrorObjectResult(json);
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        context.ExceptionHandled = true;
    }

    private class JsonErrorResponse
    {
        public string[]? Messages { get; set; }
        public object? DeveloperMessage { get; set; }
    }
}

