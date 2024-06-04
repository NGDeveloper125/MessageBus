using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessageBusDomain.Entities.Records;

namespace MessageBusService;

public class RecoveryHandler
{
    public static void SaveQueueMessages(List<QueueMessage> messages, ILogger<RecoveryHandler> logger)
    {
        string serilizedMessages = JsonSerializer.Serialize(messages);
        try
        {
            System.IO.File.WriteAllText(@".\QueueRecoveryFile.txt", serilizedMessages);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving messages to recovery file");
        }
    }

    public static List<QueueMessage> LoadQueueMessages(ILogger<RecoveryHandler> logger)
    {
        try
        {
            List<QueueMessage>? desilizedQueue = null;
            if (!System.IO.File.Exists(@".\QueueRecoveryFile.txt"))
            {
                File.Create(@".\QueueRecoveryFile.txt");
                return new List<QueueMessage>();
            }

            string serilizedMessages = System.IO.File.ReadAllText(@".\QueueRecoveryFile.txt");
            if (!string.IsNullOrEmpty(serilizedMessages) || !string.IsNullOrWhiteSpace(serilizedMessages))
            {
                desilizedQueue = JsonSerializer.Deserialize<List<QueueMessage>>(serilizedMessages);
            }
            return desilizedQueue ?? new List<QueueMessage>();
        }   
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading messages from recovery file");
            return new List<QueueMessage>();
        }
    }
}

