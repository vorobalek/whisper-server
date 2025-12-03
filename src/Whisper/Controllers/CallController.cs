using Microsoft.AspNetCore.Mvc;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Call.Infrastructure;
using Whisper.Services.GlobalCancellationToken;

namespace Whisper.Controllers;

[Route("api/v1/[controller]")]
public partial class CallController(
    ICallProcessor callProcessor,
    ILogger<CallController> logger,
    IGlobalCancellationTokenSource cancellationTokenSource) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] ICallRequest request)
    {
        try
        {
            var cancellationToken = cancellationTokenSource.Token;
            return Ok(await callProcessor.ProcessAsync(request, cancellationToken));
        }
        catch (Exception exception)
        {
            LogInternalServerError(logger, exception);
            return StatusCode(500, "Internal server error");
        }
    }

    [LoggerMessage(LogLevel.Error, "Internal server error")]
    static partial void LogInternalServerError(ILogger<CallController> logger, Exception exception);
}