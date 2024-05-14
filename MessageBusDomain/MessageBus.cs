using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using MessageBusDomain.Entities.Records;
namespace MessageBusDomain;

public class MessageBus
{
    private readonly ILogger<MessageBus> logger;
    private List<QueueMessage> queue;
    public MessageBus(ILogger<MessageBus> logger)
    {
        this.logger = logger;
        queue = new List<QueueMessage>();

    }

    public void Run(CancellationToken cancellationToken)
    {

    }

    public void HandleNewMessage(MessageWrapper messageWrapper)
    {
        if (!messageWrapper.IsValid())
        {
            return;
        }
        queue.Add(messageWrapper.GenerateQueueMessage());
    }

    public QueueInfo GetQueueInfo()
    {
        return QueueHandler.GetQueueInfo(queue);
    }

}

