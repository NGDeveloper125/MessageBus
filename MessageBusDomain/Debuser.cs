using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using System.Text;
using System.Text.Json;
using NetMQ.Sockets;
using NetMQ;
using System.Collections.Concurrent;

namespace MessageBusDomain;

public class Debuser
{
    private readonly MessageBus messageBus;
    private readonly PullSocketInfo pullSocketInfo;

    private readonly ILogger<Debuser> logger;

    public Debuser(PullSocketInfo pullSocketInfo, MessageBus messageBus, ILogger<Debuser> logger)
    {
        this.messageBus = messageBus;
        this.pullSocketInfo = pullSocketInfo;
        this.logger = logger;
    }
    private readonly ConcurrentDictionary<string, TaskCompletionSource<PulledMessage>> requestCompletionSources = new ConcurrentDictionary<string, TaskCompletionSource<PulledMessage>>();
    public void Run(CancellationToken cancellationToken)
    {
        using (var routerSocket = new RouterSocket())
        {
            routerSocket.Bind($"{pullSocketInfo.Address.AddressString}:{pullSocketInfo.Port.PortNumber}");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    
                    var clientAddress = routerSocket.ReceiveFrameBytes();
                    var empty = routerSocket.ReceiveFrameBytes(); 
                    var message = routerSocket.ReceiveFrameBytes();

                    string clientAddressString = Encoding.UTF8.GetString(clientAddress);
                    
                    var completionSource = new TaskCompletionSource<PulledMessage>();
                    requestCompletionSources[clientAddressString] = completionSource;

                    Task.Run(() =>
                    {
                        var pulledMessage = HandleNewRequestMessage(message);
                        completionSource.SetResult(pulledMessage);
                    });

                    completionSource.Task.ContinueWith(task =>
                    {
                        var pulledMessage = task.Result;
                        routerSocket.SendMoreFrame(clientAddress);
                        routerSocket.SendMoreFrameEmpty();
                        routerSocket.SendFrame(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pulledMessage)));
                        requestCompletionSources.TryRemove(clientAddressString, out _);
                    });
               
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

        }
        if (requestMsssage is null) return new PulledMessage(false, null!, PulledMessageIssue.FailedToDeSerializeMessage);
        return messageBus.HandleRequestMessage(requestMsssage);
    }
}

