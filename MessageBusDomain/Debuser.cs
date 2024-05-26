using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using System.Text;
using System.Text.Json;

namespace MessageBusDomain;

public class Debuser
{
    private readonly MessageBus messageBus;
    private readonly PullSocketInfo pullSocketInfo;

    private readonly ILogger<Debuser> logger;

    public Debuser(PullSocketInfo pullSocketInfo, MessageBus messageBus, ILogger<Debuser> logger)
    {
        this.messageBus = messageBus;
        this.pullSocketInfo = pullSocketInfo;
        this.logger = logger;
    }
    public void Run(CancellationToken cancellationToken)
    {

    }

    public PulledMessage HandleNewRequestMessage(byte[] message)
    {
        RequestMsssage? requestMsssage = null;
        try
        {
            string serializedMessage = Encoding.UTF8.GetString(message);
            requestMsssage = JsonSerializer.Deserialize<RequestMsssage>(serializedMessage);
        }
        catch 
        {

        }
        if (requestMsssage is null) return new PulledMessage(false, null!, PulledMessageIssue.FailedToDeSerializeMessage);
        return messageBus.HandleRequestMessage(requestMsssage);
    }
}

