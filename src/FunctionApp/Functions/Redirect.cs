using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FunctionApp.Functions;

public sealed class Redirect
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public Redirect(ILoggerFactory loggerFactory, IMediator mediator)
    {
        _logger = loggerFactory.CreateLogger<Redirect>();
        _mediator = mediator;
    }

    [Function("Redirect")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{route}")] HttpRequest httpRequest,
        string route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return new BadRequestResult();
        }

        var request = new RedirectionRequest(httpRequest.Host.Host, UrlEncoder.Default.Encode(route));
        _logger.LogInformation("Start processing Redirect request for {host}/{path}.", request.Host, request.Path);

        var result = await _mediator.Send(request);
        if (result.IsFailed)
        {
            _logger.LogWarning("No target URL found for redirection request {host}/{path}.", request.Host, request.Path);
            return new NotFoundResult();
        }

        string targetUrl = result.Value;

        _logger.LogInformation("Found target URL {targeUrl} for {host}/{path}.", targetUrl, request.Host, request.Path);

        return new RedirectResult(targetUrl);
    }
}
