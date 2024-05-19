
using System.Data;
using MessageBusDomain.Entities.Records;

namespace MessageBusDomain;

public static class MessageExtensions
{
    public static bool IsValid(this MessageWrapper message)
    {
        if (message == null) return false;
        if (string.IsNullOrEmpty(message.Topic) && (message.Id is null || message.Id == new Guid())) return false;
        if (string.IsNullOrEmpty(message.Payload)) return false;
        return true;
    }

    public static QueueMessage GenerateQueueMessage(this MessageWrapper message)
    {
        return new QueueMessage(message.Topic, message.Payload, message.Id, DateTime.Now);
    }
}
