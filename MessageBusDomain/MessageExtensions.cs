
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

    public static bool IsValidId(this object self)
    {
        Guid id;
        if(Guid.TryParse(self.ToString(), out id))
        {
            if(id != Guid.Empty && id != new Guid()) return true;
        }
        return false;
    }

    public static QueueMessage GenerateQueueMessage(this MessageWrapper message)
    {
        return new QueueMessage(message.Topic, message.Payload, message.Id, DateTime.Now);
    }
}
