using System.Diagnostics;

namespace BillController
{
    public class CustomMiddleware
    {
        readonly RequestDelegate next;
        public ILogger<CustomMiddleware> ILogger;
        public EventId Evenit = 1;
        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> iLogger)
        {
            this.next = next;
            ILogger = iLogger;
            
        }

        
        public async Task InvokeAsync(HttpContext context)
        {
            var sw = new Stopwatch();

            sw.Start();
            ILogger.LogInformation(Evenit.Id,
                
                $"Start Call:\nUrl: {context.Request.Path}\nClient: {context.Connection.LocalIpAddress}");
            await next(context);
            sw.Stop();
            TimeSpan elapsed = sw.Elapsed;
            ILogger.LogInformation(Evenit.Id,
                $"End Call:\nUrl: {context.Request.Path}\nClient: {context.Connection.LocalIpAddress}\nExecution Time:{elapsed.TotalMilliseconds}");
        }
    }
}
