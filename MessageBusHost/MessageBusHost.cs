
using FPMessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OOPMessageBus;

namespace MessageBus;

public class MessageBusHost : BackgroundService
{

    private OOPMessageBusService oOPMEssageBusService;
    private ILogger<OOPMessageBusService> oOPLogger;

    public MessageBusHost(IConfiguration configuration, 
                             ILogger<OOPMessageBusService> oOPLogger, 
                             OOPMessageBusService oOPMEssageBusService)
    {
        this.oOPMEssageBusService = oOPMEssageBusService;
        this.oOPLogger = oOPLogger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            oOPLogger.LogInformation("Object-Oriented Message Bus starting!");

            _ = oOPMEssageBusService.Run(stoppingToken);

            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
        catch(Exception ex) 
        {
            oOPLogger.LogError($"Object-Oriented Message Bus Fail: {ex.Message}");
            throw;
        }
    }
}