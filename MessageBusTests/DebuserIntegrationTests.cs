using Xunit;
using FluentAssertions;
using NSubstitute;
using NetMQ;
using NetMQ.Sockets;
using System.Text.Json;
using System.Text.Encodings;

using MessageBusDomain;
using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using System.Text;

namespace MessageBusTests;

public class DebuserIntegrationTests
{
    private MessageBus messageBus;
    private readonly PullSocketInfo pullSocketInfo;

    public DebuserIntegrationTests()
    {
        ILogger<MessageBus> logger = NSubstitute.Substitute.For<ILogger<MessageBus>>();
        ILogger<Debuser> debuserLogger = NSubstitute.Substitute.For<ILogger<Debuser>>();
        messageBus = new MessageBus(logger, new List<QueueMessage>());
        pullSocketInfo = new PullSocketInfo("127.0.0.1", "5555");
        Debuser debuser = new Debuser(pullSocketInfo, messageBus, debuserLogger);
        Task.Run(() => debuser.Run(new CancellationToken()));
    }

    [Fact]
    public async void PullMessage_Debus_WhenMessageRequestIsValidAndMessageExist()
    {
        messageBus.HandleNewMessage(new MessageWrapper("topic1", "paylod", null));

        using(var requestSocket = new RequestSocket())
        {
            await Task.Delay(5000);
            requestSocket.Connect($"{pullSocketInfo.Address.AddressString}:{pullSocketInfo.Port.PortNumber}");
            var requestMessage = new RequestMsssage("topic1", null);
            string requestMessageJson = JsonSerializer.Serialize(requestMessage);
            byte[] buffer = Encoding.UTF8.GetBytes(requestMessageJson);
        
            requestSocket.SendFrame(buffer);
            await Task.Delay(1000);

            var response = requestSocket.ReceiveFrameBytes();
            string responseJson = Encoding.UTF8.GetString(response);
            var pulledMessage = JsonSerializer.Deserialize<PulledMessage>(responseJson);

            pulledMessage.Should().NotBeNull();
            pulledMessage!.SuccessfullyPulled.Should().BeTrue();
            string payload = pulledMessage.Payload!;
            payload.Should().Be("paylod");
        }
    }
}


