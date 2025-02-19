using System.Text;
using System.Text.Json;
using NetMQ;
using NetMQ.Sockets;

namespace InterfaceGateway;
public static class MessageBusClient
{
    public async static Task PushMessageToBus<T>(T message, string topic, string embuserUri)
    {
        string serializedMessage = JsonSerializer.Serialize(message);
        MessageWrapper messageWrapper = new MessageWrapper(topic, serializedMessage, null);

        await SendMessage(messageWrapper, embuserUri);
    }
    
    public async static Task PushMessageToBus<T>(T message, Guid id, string embuserUri)
    {
        string serializedMessage = JsonSerializer.Serialize(message);
        MessageWrapper messageWrapper = new MessageWrapper(null!, serializedMessage, id);

        await SendMessage(messageWrapper, embuserUri);
    }

    private async static Task SendMessage(MessageWrapper message, string embuserUri)
    {
        using(var socket = new RequestSocket(embuserUri))
        {
            await Task.Delay(500);
            string serializedMessageWrapper = JsonSerializer.Serialize(message);
            socket.SendFrame(serializedMessageWrapper);
        }
    }

    public async static Task<PulledMessage> PullMessageFromBus(string topic, string embuserUri, CancellationToken cancellationToken)
    {
        using(var socket = new RequestSocket(embuserUri))
        {
            RequestMsssage request = new RequestMsssage(topic, null);
            string serializedRequest = JsonSerializer.Serialize(request);
            await Task.Delay(500);
            socket.SendFrame(serializedRequest);               

            while(!cancellationToken.IsCancellationRequested)
            {
                byte[] message;
                if(socket.TryReceiveFrameBytes(TimeSpan.FromSeconds(1), out message!))
                {
                    return TranslateByteToMessage(message);
                }
            }
            return new PulledMessage(false, string.Empty, PulledMessageIssue.NullMessage);
        }
    }

    public async static Task<PulledMessage> PullMessageFromBus(Guid id, string embuserUri, CancellationToken cancellationToken)
    {
        using(var socket = new RequestSocket(embuserUri))
        {
            RequestMsssage request = new RequestMsssage(null!, id);
            string serializedRequest = JsonSerializer.Serialize(request);
            await Task.Delay(500);
            socket.SendFrame(serializedRequest);               

            while(!cancellationToken.IsCancellationRequested)
            {
                byte[] message;
                if(socket.TryReceiveFrameBytes(TimeSpan.FromSeconds(1), out message!))
                {
                    return TranslateByteToMessage(message);
                }
            }
            return new PulledMessage(false, string.Empty, PulledMessageIssue.NullMessage);
        }
    }

    private static PulledMessage TranslateByteToMessage(byte[] message)
    {
        string serializedMessage = Encoding.UTF8.GetString(message);
        PulledMessage? pulledMessage = JsonSerializer.Deserialize<PulledMessage>(serializedMessage);
        if(pulledMessage != null) return pulledMessage;
        return new PulledMessage(false, string.Empty, PulledMessageIssue.NullMessage);
    }
}

public enum PulledMessageIssue
{
    NoIssue = 0,
    NoMessageFoundForThisTopic = 1,
    NoTopicOrIdProvided = 2,
    NoMessageFoundWithThisId = 3,
    FailedToDeSerializeMessage = 4,
    NullMessage = 5,
}
public record Address(string AddressString);
public record Port(int PortNumber);
public record MessageWrapper(string Topic, string Payload, Guid? Id);
public record QueueInfo(int QueueCount, List<string> Topics, List<Guid?> Ids);
public record QueueMessage(string Topic, string Payload, Guid? Id, DateTime EmbusTime);
public record RequestMsssage(string Topic, Guid? Id);
public record PulledMessage(bool SuccessfullyPulled, string Payload, PulledMessageIssue Issue);