
using MessageBusDomain.Entities.Records;

namespace MessageBusDomain;

public static class MessageExtensions
{
    public static bool IsValid(this MessageWrapper message)
    {
        if (message == null) return false;
        if (string.IsNullOrEmpty(message.Topic)) return false;
        if (string.IsNullOrEmpty(message.Payload)) return false;
        return true;
    }

    public static QueueMessage GenerateQueueMessage(this MessageWrapper message)
    {
        DateTime? LockedUntil = null;
        if(message.Lock)
        {
            LockedUntil = DateTime.UtcNow.AddSeconds(message.LockForInSeconds);
        }
        return new QueueMessage(message.Topic, message.Payload, message.Id, LockedUntil);
    }



}
