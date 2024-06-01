using Xunit;
using FluentAssertions;
using NSubstitute;
using MessageBusDomain;
using Microsoft.Extensions.Logging;
using MessageBusDomain.Entities.Records;
using MessageBusDomain.Entities;

namespace MessageBusTests;

public class MessageBusFacadeTests 
{
    private MessageBus messageBus;
    private readonly DebuserInfo debuserInfo;
    private readonly EmbuserInfo embuserInfo;
    private CancellationTokenSource socketsCancellationTokenSource;
    private Task debuserTask;
    private Task embuserTask;

    public MessageBusFacadeTests()
    {
        ILogger<MessageBus> logger = NSubstitute.Substitute.For<ILogger<MessageBus>>();
        ILogger<Debuser> debuserLogger = NSubstitute.Substitute.For<ILogger<Debuser>>();
        ILogger<Embuser> embuserLogger = NSubstitute.Substitute.For<ILogger<Embuser>>();
        messageBus = new MessageBus(logger, new List<QueueMessage>());
        debuserInfo = new DebuserInfo("127.0.0.1", "5555");
        embuserInfo = new EmbuserInfo("127.0.0.1", "5556");

        socketsCancellationTokenSource = new CancellationTokenSource();

        Debuser debuser = new Debuser(debuserInfo, messageBus, debuserLogger);
        debuserTask = Task.Run(() => debuser.Run(socketsCancellationTokenSource.Token));
        Embuser embuser = new Embuser(embuserInfo, messageBus, embuserLogger);
        embuserTask = Task.Run(() => embuser.Run(socketsCancellationTokenSource.Token));
    }

    private async Task DisposeAsync()
    {
        socketsCancellationTokenSource.Cancel();
        await Task.WhenAll(embuserTask, debuserTask);
        socketsCancellationTokenSource.Dispose();
    }
    [Fact]
    public async Task PushMessageToBus_EmbusMessage_WhenSentAValidMessageWithTopic()
    {
        string payload = "payload1";
        string topic = "topic1";
        await Task.Delay(2000);
        await MessageBusFacade.PushMessageToBus(payload, topic, $"{embuserInfo.Address.AddressString}:{embuserInfo.Port.PortNumber}");
        await Task.Delay(2000);
        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(1);
        await DisposeAsync();
    }

    [Fact]
    public async Task PushMessageToBus_EmbusMessage_WhenSentAValidMessageWithId()
    {
        Guid id = Guid.NewGuid();
        string payload = "payload1";
        await Task.Delay(2000);
        await MessageBusFacade.PushMessageToBus(payload, id, $"{embuserInfo.Address.AddressString}:{embuserInfo.Port.PortNumber}");
        await Task.Delay(2000);
        QueueInfo queueInfo = messageBus.GetQueueInfo();

        queueInfo.QueueCount.Should().Be(1);
        await DisposeAsync();
    }

    [Fact]
    public async Task PullMessageFromBus_GetIssueMessage_WhenNoTopicOrIdIsProvided()
    {
        string payload = "payload1";
        string topic = "topic1";
        messageBus.HandleNewMessage(new MessageWrapper(topic, payload, null));

        await Task.Delay(2000);
        PulledMessage pulledMessage = await MessageBusFacade.PullMessageFromBus(null, $"{debuserInfo.Address.AddressString}:{debuserInfo.Port.PortNumber}", new CancellationToken());
        await Task.Delay(2000);

        pulledMessage.SuccessfullyPulled.Should().BeFalse();
        pulledMessage.Issue.Should().Be(PulledMessageIssue.NoTopicOrIdProvided);
        await DisposeAsync();
    }

    [Fact]
    public async Task PullMessageFromBus_GetIssueMessage_WhenNoMsgForTheTopic()
    {
        string payload = "payload1";
        string topic = "topic1";
        messageBus.HandleNewMessage(new MessageWrapper(topic, payload, null));

        await Task.Delay(2000);
        PulledMessage pulledMessage = await MessageBusFacade.PullMessageFromBus("topic2", $"{debuserInfo.Address.AddressString}:{debuserInfo.Port.PortNumber}", new CancellationToken());
        await Task.Delay(2000);

        pulledMessage.SuccessfullyPulled.Should().BeFalse();
        pulledMessage.Issue.Should().Be(PulledMessageIssue.NoMessageFoundForThisTopic);
        await DisposeAsync();
    }

    [Fact]
    public async Task PullMessageFromBus_GetIssueMessage_WhenNoMsgWithTheIdOnQueue()
    {
        string payload = "payload1";
        Guid id = Guid.NewGuid();

        messageBus.HandleNewMessage(new MessageWrapper(null, payload, id));

        await Task.Delay(2000);
        PulledMessage pulledMessage = await MessageBusFacade.PullMessageFromBus(Guid.NewGuid(), $"{debuserInfo.Address.AddressString}:{debuserInfo.Port.PortNumber}", new CancellationToken());
        await Task.Delay(2000);

        pulledMessage.SuccessfullyPulled.Should().BeFalse();
        pulledMessage.Issue.Should().Be(PulledMessageIssue.NoMessageFoundWithThisId);
        await DisposeAsync();
    }

    [Fact]
    public async Task PullMessageFromBus_PullMessage_WhenAValidRequestForAnExistingTopic()
    {
        string payload = "payload1";
        string topic = "topic1";

        messageBus.HandleNewMessage(new MessageWrapper(topic, payload, null));

        await Task.Delay(2000);
        PulledMessage pulledMessage = await MessageBusFacade.PullMessageFromBus(topic, $"{debuserInfo.Address.AddressString}:{debuserInfo.Port.PortNumber}", new CancellationToken());
        await Task.Delay(2000);

        pulledMessage.SuccessfullyPulled.Should().BeTrue();
        pulledMessage.Payload.Should().Be(payload);
        await DisposeAsync();
    }

    [Fact]
    public async Task PullMessageFromBus_PullMessage_WhenAValidRequestForAnExistingId()
    {
        string payload = "payload1";
        Guid id = Guid.NewGuid();

        messageBus.HandleNewMessage(new MessageWrapper(null, payload, id));

        await Task.Delay(2000);
        PulledMessage pulledMessage = await MessageBusFacade.PullMessageFromBus(id, $"{debuserInfo.Address.AddressString}:{debuserInfo.Port.PortNumber}", new CancellationToken());
        await Task.Delay(2000);

        pulledMessage.SuccessfullyPulled.Should().BeTrue();
        pulledMessage.Payload.Should().Be(payload);
        await DisposeAsync();
    }
}
