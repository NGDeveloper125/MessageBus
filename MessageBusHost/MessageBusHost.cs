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
    private readonly ILogger<MessageBusDomain.MessageBus> messageBusLogger;
    private readonly ILogger<Embuser> embuserLogger;
    private readonly ILogger<Debuser> debuserLogger;
    private Task? embuserTask;
    private Task? debuserTask;

    public MessageBusHost(ILogger<MessageBusDomain.MessageBus> messageBusLogger, ILogger<Embuser> embuserLogger, ILogger<Debuser> debuserLogger, IConfiguration configuration)
    {
        this.messageBusLogger = messageBusLogger;
        this.embuserLogger = embuserLogger;
        this.debuserLogger = debuserLogger;

        messageBus = new MessageBusDomain.MessageBus(messageBusLogger, null!);
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {    
            EmbuserInfo embuserInfo = new EmbuserInfo(configuration["Embuser:Address"]!, configuration["Embuser:Port"]!);
            DebuserInfo debuserInfo = new DebuserInfo(configuration["Debuser:Address"]!, configuration["Debuser:Port"]!);

            Embuser embuser = new Embuser(embuserInfo, messageBus, embuserLogger);
            Debuser debuser = new Debuser(debuserInfo, messageBus, debuserLogger);

            messageBusLogger.LogInformation("Message Bus starting...");   
            messageBus.Run(stoppingToken);  

            messageBusLogger.LogInformation($"Embuser starting on {embuserInfo.Address.AddressString}:{embuserInfo.Port.PortNumber}");   
            embuserTask = Task.Run(() => { embuser.Run(stoppingToken); });

            messageBusLogger.LogInformation($"Debuser starting on {debuserInfo.Address.AddressString}:{debuserInfo.Port.PortNumber}");   
            debuserTask = Task.Run(() => { debuser.Run(stoppingToken); });

            messageBusLogger.LogInformation("Message Bus Running");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1, stoppingToken);
            }
        }
        catch(Exception ex) 
        {
            messageBusLogger.LogError($"MessageBus crashed: {ex.Message}");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        messageBusLogger.LogInformation("Message Bus stopping...");

        await Task.WhenAll(embuserTask!, debuserTask!);

        messageBusLogger.LogInformation("Message Bus stopped.");
        await base.StopAsync(cancellationToken);
    }
}