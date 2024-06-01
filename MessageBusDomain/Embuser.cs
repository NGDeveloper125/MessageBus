using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using System.Text.Json;
using System.Text;
using NetMQ.Sockets;
using NetMQ;
using System.Net.Sockets;

namespace MessageBusDomain;

public class Embuser
{
    private readonly MessageBus messageBus;
    private readonly EmbuserInfo embuserInfo;
    private readonly ILogger<Embuser> logger;

    public Embuser(EmbuserInfo embuserInfo, MessageBus messageBus, ILogger<Embuser> logger)
    {
        this.messageBus = messageBus;
        this.embuserInfo = embuserInfo;
        this.logger = logger;
    }
    // public Task Run(CancellationToken cancellationToken)
    // {
    //     using (var socket = new RouterSocket($"{embuserInfo.Address.AddressString}:{embuserInfo.Port.PortNumber}"))
    //     {
    //         while (!cancellationToken.IsCancellationRequested)
    //         {
    //             try
    //             {
    //                 RoutingKey routingKey = socket.ReceiveRoutingKey();
    //                 string msg = socket.ReceiveFrameString();
    //                 byte[] message = socket.ReceiveFrameBytes();
    //                 HandleNewMessage(message);
    //             }
    //             catch (Exception ex)
    //             {
    //                 logger.LogError(ex, "Error while receiving new message");
    //             }
    //         }
    //         return Task.CompletedTask;
    //     }
    // }

    public void Run(CancellationToken cancellationToken)
    {
        using (var socket = new RouterSocket($"{embuserInfo.Address.AddressString}:{embuserInfo.Port.PortNumber}"))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    RoutingKey routingKey;
                    if (socket.TryReceiveRoutingKey(TimeSpan.FromSeconds(1), ref routingKey))
                    {
                        string msg = socket.ReceiveFrameString();
                        byte[] message = socket.ReceiveFrameBytes();
                        HandleNewMessage(message);
                    }
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

