using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using System.Text.Json;
using System.Text;

namespace MessageBusDomain;

public class PushSocket
{
    private readonly MessageBus messageBus;
    private readonly PushSocketInfo pushSocketInfo;

    private readonly ILogger<PushSocket> logger;

    public PushSocket(PushSocketInfo pushSocketInfo, MessageBus messageBus, ILogger<PushSocket> logger)
    {
        this.messageBus = messageBus;
        this.pushSocketInfo = pushSocketInfo;
        this.logger = logger;
    }
    public void Run(CancellationToken cancellationToken)
    {

    }

    public void HandleNewMessage(byte[] buffer)
    {
        Task.Run(() =>
        {
            try
            {
                string serializedMessage = Encoding.UTF8.GetString(buffer);
                MessageWrapper? messageWrapper = JsonSerializer.Deserialize<MessageWrapper>(serializedMessage);
                if (messageWrapper == null) return;
                messageBus.HandleNewMessage(messageWrapper);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while handling new message");
            }
        });
    }
}

