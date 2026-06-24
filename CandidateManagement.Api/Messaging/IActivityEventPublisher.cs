using CandidateManagement.Messaging;

namespace CandidateManagement.Api.Messaging
{
    public interface IActivityEventPublisher
    {
        Task PublishAsync(ActivityEvent activityEvent);
    }
}
