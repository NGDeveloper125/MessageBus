using InterfaceGateway;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/getbusdata", async (string address) => {
    return await GetBusDataHandler.Handle(address);
});

app.Run();
