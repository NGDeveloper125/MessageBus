using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MessageBusDomain;

namespace MessageBus;

public class MessageBusHost : BackgroundService
{

    private readonly MessageBusDomain.MessageBus messageBus;
    private readonly ILogger<MessageBusDomain.MessageBus> logger;

    public MessageBusHost(ILogger<MessageBusDomain.MessageBus> logger, MessageBusDomain.MessageBus messageBus)
    {
        this.messageBus = messageBus;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Message Bus starting!");

            messageBus.Run(stoppingToken);

            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
        catch(Exception ex) 
        {
            logger.LogError($"MessageBus crashed: {ex.Message}");
            throw;
        }
    }
}