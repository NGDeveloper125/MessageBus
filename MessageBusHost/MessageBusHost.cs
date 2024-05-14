using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MessageBusDomain;
using MessageBusDomain.Entities;

namespace MessageBus;

public class MessageBusHost : BackgroundService
{
    private readonly IConfiguration configuration;
    private readonly MessageBusDomain.MessageBus messageBus;
    private readonly ILogger<MessageBusDomain.MessageBus> logger;
    private readonly ILogger<PushSocket> pushSocketLogger;
    private readonly ILogger<PullSocket> pullSocketLogger;

    public MessageBusHost(ILogger<MessageBusDomain.MessageBus> logger, IConfiguration configuration, MessageBusDomain.MessageBus messageBus)
    {
        this.messageBus = messageBus;
        this.logger = logger;
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {    
            PushSocketInfo pushSocketInfo = new PushSocketInfo(configuration["PushSocket:Address"], configuration["PushSocket:Port"]);
            PullSocketInfo pullSocketInfo = new PullSocketInfo(configuration["PullSocket:Address"], configuration["PullSocket:Port"]);

            PushSocket pushSocket = new PushSocket(pushSocketInfo, messageBus, pushSocketLogger);
            PullSocket pullSocket = new PullSocket(pullSocketInfo, messageBus, pullSocketLogger);

            logger.LogInformation("Message Bus starting..");   
            messageBus.Run(stoppingToken);  

            logger.LogInformation($"Push Socket starting on  {pushSocketInfo.Address} port: {pushSocketInfo.Port}");   
            Task.Run(() => { pushSocket.Run(stoppingToken); });

            logger.LogInformation($"Pull Socket starting on {pullSocketInfo.Address} port: {pullSocketInfo.Port}");   
            Task.Run(() => { pullSocket.Run(stoppingToken); });

            logger.LogInformation("Message Bus Running");   
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