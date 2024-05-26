using Xunit;
using FluentAssertions;
using NSubstitute;
using MessageBusDomain;
using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using NetMQ;
using NetMQ.Sockets;
using System.Text.Json;
using System.Text;

public class EmbuserIntegrationTests
{
    private MessageBus messageBus;
    private readonly PushSocketInfo pushSocketInfo;

    public EmbuserIntegrationTests()
    {
        ILogger<MessageBus> logger = NSubstitute.Substitute.For<ILogger<MessageBus>>();
        ILogger<Embuser> embuserLogger = NSubstitute.Substitute.For<ILogger<Embuser>>();
        messageBus = new MessageBus(logger, new List<QueueMessage>());
        pushSocketInfo = new PushSocketInfo("127.0.0.1", "5555");
        Embuser embuser = new Embuser(pushSocketInfo, messageBus, embuserLogger);
        Task.Run(() => embuser.Run(new CancellationToken()));
    }

    //Need to be Debug
    // [Fact]
    // public async void PushMessage_Embus_WhenMessageIsValid()
    // {
    //     using (NetMQSocket socket = new PushSocket())
    //     {
    //         socket.Connect($"tcp://{pushSocketInfo.Address.AddressString}:{pushSocketInfo.Port.PortNumber}");
    //         var messageWrapper = new MessageWrapper("Topic1", "payload", null);
    //         var serializedMessage = JsonSerializer.Serialize(messageWrapper);
    //         var buffer = Encoding.UTF8.GetBytes(serializedMessage);

    //         socket.SendFrame(buffer);
    //     }

    //     QueueInfo queueInfo = messageBus.GetQueueInfo();

    //     queueInfo!.QueueCount.Should().Be(1);
    // }
}
