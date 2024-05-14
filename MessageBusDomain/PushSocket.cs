using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;

namespace MessageBusDomain;

public class PushSocket
{
    private readonly MessageBus messageBus;
    private readonly PushSocketInfo pushSocketInfo;

    private readonly ILogger<PushSocket> logger;

    public PushSocket(PushSocketInfo pushSocketInfo, MessageBus messageBus, ILogger<PushSocket> logger)
    {
        this.messageBus = messageBus;
        this.pushSocketInfo = pushSocketInfo;
        this.logger = logger;
    }
    public void Run(CancellationToken cancellationToken)
    {

    }
}

