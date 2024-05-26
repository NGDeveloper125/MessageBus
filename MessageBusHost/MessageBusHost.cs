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
    private readonly ILogger<Embuser> EmbuserLogger;
    private readonly ILogger<Debuser> DebuserLogger;

    public MessageBusHost(ILogger<MessageBusDomain.MessageBus> logger, IConfiguration configuration)
    {
        this.logger = logger;
        messageBus = new MessageBusDomain.MessageBus(logger, null);
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {    
            PushSocketInfo pushSocketInfo = new PushSocketInfo(configuration["PushSocket:Address"], configuration["PushSocket:Port"]);
            PullSocketInfo pullSocketInfo = new PullSocketInfo(configuration["PullSocket:Address"], configuration["PullSocket:Port"]);

            Embuser embuser = new Embuser(pushSocketInfo, messageBus, EmbuserLogger);
            Debuser debuser = new Debuser(pullSocketInfo, messageBus, DebuserLogger);

            logger.LogInformation("Message Bus starting..");   
            messageBus.Run(stoppingToken);  

            logger.LogInformation($"Push Socket starting on  {pushSocketInfo.Address} port: {pushSocketInfo.Port}");   
            Task.Run(() => { embuser.Run(stoppingToken); });

            logger.LogInformation($"Pull Socket starting on {pullSocketInfo.Address} port: {pullSocketInfo.Port}");   
            Task.Run(() => { debuser.Run(stoppingToken); });

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