﻿using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using System.Text;
using System.Text.Json;
using NetMQ.Sockets;
using NetMQ;
using System.Collections.Concurrent;

namespace MessageBusDomain;

public class Debuser
{
    private readonly MessageBus messageBus;
    private readonly DebuserInfo debuserInfo;

    private readonly ILogger<Debuser> logger;

    public Debuser(DebuserInfo debuserInfo, MessageBus messageBus, ILogger<Debuser> logger)
    {
        this.messageBus = messageBus;
        this.debuserInfo = debuserInfo;
        this.logger = logger;
    }
    private readonly ConcurrentDictionary<RoutingKey, TaskCompletionSource<PulledMessage>> requestCompletionSources = new ConcurrentDictionary<RoutingKey, TaskCompletionSource<PulledMessage>>();
    
    public void Run(CancellationToken cancellationToken)
    {
        using (var socket = new RouterSocket($"{debuserInfo.Address.AddressString}:{debuserInfo.Port.PortNumber}"))
        {
            logger.LogInformation("Debuser is now listning for messages");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    RoutingKey routingKey = new RoutingKey();
                    if(socket.TryReceiveRoutingKey(TimeSpan.FromSeconds(1), ref routingKey))
                    {     
                        logger.LogDebug("New request message reseived");
                        var clientAddress = socket.ReceiveFrameBytes();
                        var message = socket.ReceiveFrameBytes(); 
                        var completionSource = new TaskCompletionSource<PulledMessage>();
                        requestCompletionSources[routingKey] = completionSource;

                        Task.Run(() =>
                        {
                            var pulledMessage = HandleNewRequestMessage(message);
                            completionSource.SetResult(pulledMessage);
                        });

                        completionSource.Task.ContinueWith(task =>
                        {
                            var pulledMessage = task.Result;
                            socket.SendMoreFrame(routingKey);
                            socket.SendMoreFrameEmpty();
                            socket.SendFrame(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pulledMessage)));
                            requestCompletionSources.TryRemove(routingKey, out _);
                            logger.LogDebug("Request message handled and response sent");
                        });
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to handle request message");
                }
            }
        }
    }

    public PulledMessage HandleNewRequestMessage(byte[] message)
    {
        RequestMsssage? requestMsssage = null;
        try
        {
            string serializedMessage = Encoding.UTF8.GetString(message);
            requestMsssage = JsonSerializer.Deserialize<RequestMsssage>(serializedMessage);
        }
        catch 
        {
            logger.LogDebug("Failed to deserialize message");
        }
        if (requestMsssage is null) 
        {
            logger.LogDebug("Message was not valid");
            return new PulledMessage(false, null!, PulledMessageIssue.FailedToDeSerializeMessage);
        }
        return messageBus.HandleRequestMessage(requestMsssage);
    }
}

