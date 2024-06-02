using MessageBusDomain.Entities.Records;
using System.Reflection.Metadata.Ecma335;

namespace MessageBusDomain;

public static class QueueHandler
{
    public static QueueMessage? GetNextMessageByTopic(List<QueueMessage> queue, string topic) =>
                                queue.Where(qm => qm.Topic == topic)
                                     .OrderBy(qm => qm.EmbusTime)
                                     .FirstOrDefault();
    

    public static QueueInfo GetQueueInfo(List<QueueMessage> queue) =>
                    new QueueInfo(queue.Count, GetUniqeTopics(queue), GetUniqeIds(queue));
    

    internal static QueueMessage? GetMessageById(List<QueueMessage> queue, Guid? id) =>
                                    queue.Where(qm => qm.Id == id).FirstOrDefault();
    
    private static List<string> GetUniqeTopics(List<QueueMessage> queue) => 
                                    queue.Distinct().Select(qm => qm.Topic).ToList();
    
    private static List<Guid?> GetUniqeIds(List<QueueMessage> queue) => 
                                    queue.Distinct().Select(qm => qm.Id).ToList();

    public static List<QueueMessage> RemoveMessageFromQueue(List<QueueMessage> queue, QueueMessage message)
    {
        queue.Remove(message);
        return new List<QueueMessage>(queue);
    }
                                    
}