using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MessageBusDomain;

public class MessageBus
{
    private readonly ILogger<MessageBus> logger;
    private readonly IConfiguration configuration;
    public MessageBus(ILogger<MessageBus> logger, IConfiguration configuration)
    {

        this.logger = logger;
        this.configuration = configuration;
    }

    public void Run(CancellationToken cancellationToken)
    {
        string pushSocketAddress = "";
        string pullSocketAddress = "";
        PushSocket pushSocket = new PushSocket();
        PullSocket pullSocket = new PullSocket();
        Task.Run(() => { pushSocket.Run(pushSocketAddress); });
        Task.Run(() => { pullSocket.Run(pullSocketAddress); });



    }
}

