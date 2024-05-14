using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;

namespace MessageBusDomain;

public class PullSocket
{
    private readonly MessageBus messageBus;
    private readonly PullSocketInfo pullSocketInfo;

    private readonly ILogger<PullSocket> logger;

    public PullSocket(PullSocketInfo pullSocketInfo, MessageBus messageBus, ILogger<PullSocket> logger)
    {
        this.messageBus = messageBus;
        this.pullSocketInfo = pullSocketInfo;
        this.logger = logger;
    }
    public void Run(CancellationToken cancellationToken)
    {

    }
}

