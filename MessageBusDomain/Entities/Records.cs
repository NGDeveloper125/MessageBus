
namespace MessageBusDomain.Entities.Records;

public record Address(string AddressString);
public record Port(int PortNumber);
public record MessageWrapper(string Topic, string Payload, Guid Id = new Guid(), bool Lock = false, int LockForInSeconds = 0);
public record QueueInfo(int QueueCount, List<string> Topics);
public record QueueMessage(string Topic, string Payload, Guid? Id, DateTime? LockedUntil);