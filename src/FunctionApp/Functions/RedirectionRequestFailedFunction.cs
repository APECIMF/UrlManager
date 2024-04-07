using Azure.Storage.Queues.Models;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using TheGnouCommunity.UrlManager.Application.Commands;
using TheGnouCommunity.UrlManager.Domain.Messaging;

namespace FunctionApp.Functions;

public sealed class RedirectionRequestFailedFunction
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public RedirectionRequestFailedFunction(ILoggerFactory loggerFactory, IMediator mediator)
    {
        _logger = loggerFactory.CreateLogger<RedirectionRequestFailedFunction>();
        _mediator = mediator;
    }

    [Function(nameof(RedirectionRequestFailedFunction))]
    public async Task Run(
        [QueueTrigger("redirection-request-failed")] QueueMessage queueMessage)
    {
        var message = JsonSerializer.Deserialize<RedirectionRequestFailed>(
            queueMessage.Body.ToString());

        await _mediator.Send(new CollectAnalysticsRequest(message.Host, message.Path, message.IPAddress, message.RequestTime, message.Errors));
    }
}
