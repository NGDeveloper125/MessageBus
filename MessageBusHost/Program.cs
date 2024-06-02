using MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;

IConfiguration configuration = new ConfigurationBuilder()
                                .AddJsonFile("MessageBusHostConfiguration.json")
                                .Build();

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

IHost host = Host.CreateDefaultBuilder()
                 .ConfigureAppConfiguration(builder =>
                 {
                     builder.Sources.Clear();
                     builder.AddConfiguration(configuration);
                 })
                 .UseWindowsService(options =>
                 {
                     options.ServiceName = "MessageBus";
                 })
                 .ConfigureServices(services =>
                 {
                     services.AddHostedService<MessageBusHost>();
                 })
                 .UseSerilog()
                 .Build();


await host.StartAsync();
while (true)
{
    await Task.Delay(1);
}