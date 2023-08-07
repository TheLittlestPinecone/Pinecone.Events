namespace Pinecone.EventPublisher;

public interface IPublisher<TEvent>
{
    Task PublishAsync(TEvent @event);
}
