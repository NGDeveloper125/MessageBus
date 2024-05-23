using System.Net;

using MessageBusDomain.Entities.Records;

namespace MessageBusDomain.Entities;

public class PullSocketInfo
{
    public Address Address { get; init; }
    public Port Port { get; init; }

    public PullSocketInfo(string address, string port)
    {
        if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
        if (string.IsNullOrEmpty(port)) throw new ArgumentNullException(nameof(port));

        if (!IPAddress.TryParse(address, out var ipAddress)) throw new ArgumentException("Invalid IP address", nameof(address));
        if (!int.TryParse(port, out var portNumber) || int.Parse(port) < 1 || int.Parse(port) > 65535) throw new ArgumentException("Invalid port number", nameof(port));
        string fullAddress = $@"tcp://{address}";
        Address = new Address(fullAddress);
        Port = new Port(portNumber);
    }
}