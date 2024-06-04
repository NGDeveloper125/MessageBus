
using System.Text;
using System.Text.Json;
using MessageBusDomain.Entities;
using MessageBusDomain.Entities.Records;
using NetMQ;
using NetMQ.Sockets;

namespace MessageBusTests;
public static class MessageBusFacade
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