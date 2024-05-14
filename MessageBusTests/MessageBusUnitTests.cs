using Xunit;
using NSubstitute;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MessageBusDomain;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;

namespace MessageBusTests;

public class MessageBusUnitTests
{
    [Fact]
    public void HandleNewMessage_DontEmbusMessage_WhenMessageHasEmptyTopic()
    {
        ILogger<MessageBus> logger = NSubstitute.Substitute.For<ILogger<MessageBus>>();
        MessageBus messageBus = new MessageBus(logger);
        MessageWrapper messageWrapper = new MessageWrapper("", "payload");

        messageBus.HandleNewMessage(messageWrapper);
        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(0);
    }

    [Fact]
    public void HandleNewMessage_DontEmbusMessage_WhenMessageHasEmptyPayload()
    {
        ILogger<MessageBus> logger = NSubstitute.Substitute.For<ILogger<MessageBus>>();
        MessageBus messageBus = new MessageBus(logger);
        MessageWrapper messageWrapper = new MessageWrapper("TestTopic", "");

        messageBus.HandleNewMessage(messageWrapper);
        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(0);
    }

    
    [Fact]
    public void HandleNewMessage_EmbusMessage_WhenMessageIsValid()
    {
        ILogger<MessageBus> logger = NSubstitute.Substitute.For<ILogger<MessageBus>>();
        MessageBus messageBus = new MessageBus(logger);
        MessageWrapper messageWrapper = new MessageWrapper("TestTopic", "TestPayload");

        messageBus.HandleNewMessage(messageWrapper);
        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(1);
        queueInfo.Topics.Should().Contain("TestTopic");

    }
}