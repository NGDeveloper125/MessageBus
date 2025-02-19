
namespace InterfaceGateway;

public static class GetBusDataHandler 
{

    public static async Task<string> Handle(string address)
    {
        if(!ValidateAddress(address))
        {
            return $"Address {address} Not Valid";
        }

        Guid id = Guid.NewGuid();
        int retries = 0;
        string payload = "MessageBus_State_Data";
        await MessageBusClient.PushMessageToBus(payload, id, $"{address}:5001");
        
        int timeoutInMilliseconds = 5000;
        while(retries < 3)
        {
            using var cts = new CancellationTokenSource(timeoutInMilliseconds);
            retries++;
            PulledMessage pulledMessage = await MessageBusClient.PullMessageFromBus(null!, $"{address}:5001", cts.Token);
            if(pulledMessage.SuccessfullyPulled)
            {
                return pulledMessage.Payload;
            }
    
            switch(pulledMessage.Issue) 
            { 
               case PulledMessageIssue.NullMessage: 
                    return "Failed to find bus";
               case PulledMessageIssue.NoMessageFoundWithThisId: 
                    if(retries < 3) break;
                    return "Failed to get data from bus"; 
               case PulledMessageIssue.FailedToDeSerializeMessage: 
                    return "Failed to handle bus data";
               case PulledMessageIssue.NoTopicOrIdProvided: 
                    return "InterfaceGateway failed to provide id";
               default: return $"Unexpected issue: {pulledMessage.Issue}";
            };
        }

        return "Unknown issue";
    }

    private static bool ValidateAddress(string address)
    {
        return System.Net.IPAddress.TryParse(address, out _);
    }
}