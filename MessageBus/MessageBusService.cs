
using FPMessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OOPMessageBus;

namespace MessageBus;

public class MessageBusService : BackgroundService
{
    //Only one of each is needed - depande on which message bus you are running
    // private FPMessageBusService fPMessageBusService;
    private OOPMessageBusService oOPMEssageBusService;
    // private ILogger<FPMessageBusService> fpLogger;
    private ILogger<OOPMessageBusService> oOPLogger;

    public MessageBusService(IConfiguration configuration, 
                             //ILogger<FPMessageBusService> fPLogger, 
                             ILogger<OOPMessageBusService> oOPLogger, 
                             //FPMessageBusService fPMessageBusService,
                             OOPMessageBusService oOPMEssageBusService)
    {
        //this.fPMessageBusService = fPMessageBusService;
        this.oOPMEssageBusService = oOPMEssageBusService;
        //this.fpLogger = fPLogger;
        this.oOPLogger = oOPLogger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // fpLogger.LogInformation("Functioanl Message Bus starting!");
            oOPLogger.LogInformation("Object-Oriented Message Bus starting!");

            // _ = fPMessageBusService.Run(stoppingToken);
            _ = oOPMEssageBusService.Run(stoppingToken);

            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
        catch(Exception ex) 
        {
            // fpLogger.LogError($"Functioanl Message Bus Fail: {ex.Message}");
            oOPLogger.LogError($"Object-Oriented Message Bus Fail: {ex.Message}");
            throw;
        }
    }
}