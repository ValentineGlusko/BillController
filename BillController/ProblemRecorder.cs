using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BillController
{
    public   class ProblemRecorder(ILogger<ProblemRecorder> logger)
    {
        private ILogger _logger { get; } = logger;

        public void SaveProblem(ProblemDetails details, Func<ILogger, IDisposable> functionForScope)
        {
            using (var scope = functionForScope(_logger)) // Теперь scope создаётся корректно
            {
                _logger.LogWarning($"Error:\n Title:{details.Title}\n,Details:{details.Detail}\n,Instance:{details.Instance}\nExtensions Values:{details.Extensions.Values}\nType:{details.Type}");
            }
        }
    }
}
