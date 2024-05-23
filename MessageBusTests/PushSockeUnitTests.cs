using Xunit;
using FluentAssertions;
using MessageBusDomain;
using Castle.Core.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities.Records;
using MessageBusDomain.Entities;
using System.Text.Json;
using System.Text;

public class PushSockeUnitTests
{
    private MessageBus messageBus;
    private PushSocket pushSocket;

    public PushSockeUnitTests()
    {
        ILogger<MessageBus> logger = NSubstitute.Substitute.For<ILogger<MessageBus>>();
        ILogger<PushSocket> pushSocketLogger = NSubstitute.Substitute.For<ILogger<PushSocket>>();
        messageBus = new MessageBus(logger, new List<QueueMessage>());
        PushSocketInfo pushSocketInfo = new PushSocketInfo("0.0.0.0", "5555");
        pushSocket = new PushSocket(pushSocketInfo, messageBus, pushSocketLogger);
    }

    [Fact]
    public void HandleNewMessage_DontAddMessageToQueue_WhenMessageHaveNoTopicOrId()
    {
        MessageWrapper messageWrapper = new MessageWrapper(null!, "payload", null!);
        string serializedMessage = JsonSerializer.Serialize(messageWrapper);
        byte[] buffer = Encoding.UTF8.GetBytes(serializedMessage);

        pushSocket.HandleNewMessage(buffer);
        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(0);
    }

    [Fact]
    public void HandleNewMessage_DontAddMessageToQueue_WhenMessageHaveNoPayload()
    {
        MessageWrapper messageWrapper = new MessageWrapper("Topic", null!, null!);
        string serializedMessage = JsonSerializer.Serialize(messageWrapper);
        byte[] buffer = Encoding.UTF8.GetBytes(serializedMessage);

        pushSocket.HandleNewMessage(buffer);
        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(0);
    }

    [Fact]
    public async void HandleNewMessage_DontAddMessageToQueue_WhenMessageIdAlreadyExistInQueue()
    {
        Guid messageId = Guid.NewGuid();
        MessageWrapper messageWrapper = new MessageWrapper(null!, "payload", messageId);
        messageBus.HandleNewMessage(messageWrapper);
        string serializedMessage = JsonSerializer.Serialize(messageWrapper);
        byte[] buffer = Encoding.UTF8.GetBytes(serializedMessage);

        pushSocket.HandleNewMessage(buffer);
        await Task.Delay(1000);

        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(1);
    }

    [Fact]
    public async void HandleNewMessage_MessageToQueue_WhenMessageIsValid()
    {
        MessageWrapper messageWrapper = new MessageWrapper(null!, "payload", Guid.NewGuid());
        string serializedMessage = JsonSerializer.Serialize(messageWrapper);
        byte[] buffer = Encoding.UTF8.GetBytes(serializedMessage);

        pushSocket.HandleNewMessage(buffer);
        await Task.Delay(1000);

        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(1);
    }

}