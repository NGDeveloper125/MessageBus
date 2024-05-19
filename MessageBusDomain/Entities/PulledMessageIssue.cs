
namespace MessageBusDomain.Entities;

public enum PulledMessageIssue
{
    NoIssue = 0,
    NoMessageFoundForThisTopic = 1,
    NoTopicOrIdProvided = 2,
    NoMessageFoundWithThisId = 3
}