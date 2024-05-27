using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using System.Text.Json;
using System.Text;
using NetMQ.Sockets;
using NetMQ;

namespace MessageBusDomain;

public class Embuser
{
    private readonly MessageBus messageBus;
    private readonly PushSocketInfo pushSocketInfo;
    private readonly ILogger<Embuser> logger;

    public Embuser(PushSocketInfo pushSocketInfo, MessageBus messageBus, ILogger<Embuser> logger)
    {
        this.messageBus = messageBus;
        this.pushSocketInfo = pushSocketInfo;
        this.logger = logger;
    }
    public void Run(CancellationToken cancellationToken)
    {
        using (var socket = new PullSocket())
        {
            socket.Bind($"{pushSocketInfo.Address.AddressString}:{pushSocketInfo.Port.PortNumber}");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    byte[] buffer = socket.ReceiveFrameBytes();
                    HandleNewMessage(buffer);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while receiving new message");
                }
            }
        }
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

