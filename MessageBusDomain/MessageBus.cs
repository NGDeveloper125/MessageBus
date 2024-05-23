using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using MessageBusDomain.Entities.Records;
using MessageBusDomain.Entities;
namespace MessageBusDomain;

public class MessageBus
{
    private readonly ILogger<MessageBus> logger;
    private List<QueueMessage> queue;
    public MessageBus(ILogger<MessageBus> logger, List<QueueMessage> previousMessages)
    {
        this.logger = logger;
        queue = new List<QueueMessage>();
        if (previousMessages != null)
        {
            queue.AddRange(previousMessages);
        }
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
        QueueInfo queueInfo = QueueHandler.GetQueueInfo(queue);
        if(queueInfo.Ids.Contains(messageWrapper.Id))
        {
            return;
        }
        queue.Add(messageWrapper.GenerateQueueMessage());
    }

    public PulledMessage HandleRequestMessage(RequestMsssage requestMsssage)
    {
        QueueMessage? queueMessage;
        if ((requestMsssage.Topic == null || requestMsssage.Topic == "") && requestMsssage.Id == null)
        {
            return new PulledMessage(false, null!, PulledMessageIssue.NoTopicOrIdProvided);
        }

        if (requestMsssage.Id is not null)
        {
            queueMessage = QueueHandler.GetMessageById(queue, requestMsssage.Id);
            if (queueMessage != null) return new PulledMessage(true, queueMessage.Payload, PulledMessageIssue.NoIssue);
            return new PulledMessage(false, null!, PulledMessageIssue.NoMessageFoundWithThisId);
        }

        queueMessage = QueueHandler.GetNextMessageByTopic(queue, requestMsssage.Topic!);

        if(queueMessage != null)
        {

            return new PulledMessage(true, queueMessage.Payload, PulledMessageIssue.NoIssue);
        }
        
        return new PulledMessage(false, null, PulledMessageIssue.NoMessageFoundForThisTopic);
    }

    public QueueInfo GetQueueInfo()
    {
        return QueueHandler.GetQueueInfo(queue);
    }

}

