using FPMessageBus;
using MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OOPMessageBus;
using Serilog;

IConfiguration configuration = new ConfigurationBuilder()
                                .AddJsonFile("")
                                .Build();

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(configuration["Serilog:LogFilePath"]!)
                .CreateLogger();

IHost host = Host.CreateDefaultBuilder ()
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
                    services.AddHostedService<MessageBusService>();
                    // Only one of this is needed, depande on which mesage bus you want to use.
                    services.AddSingleton<FPMessageBusService>();
                    services.AddSingleton<OOPMessageBusService>();
                 })
                 .UseSerilog()
                 .Build();

                