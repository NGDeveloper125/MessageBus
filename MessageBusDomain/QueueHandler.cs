using MessageBusDomain.Entities.Records;

namespace MessageBusDomain;

public static class QueueHandler
{
    public static QueueInfo GetQueueInfo(List<QueueMessage> queue)
    {
        return new QueueInfo(queue.Count, GetUniqeTopics(queue));
    }

    private static List<string> GetUniqeTopics(List<QueueMessage> queue)
    {
        return queue.Distinct().Select(qm => qm.Topic).ToList();
    }

}