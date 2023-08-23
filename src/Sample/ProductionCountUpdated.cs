using Pinecone.EventSubscriber;

namespace Sample;

public class ProductCountUpdated
{
    public string Item { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProductionCountUpdatedSubscriber : ISubscriber<ProductCountUpdated>
{
    public Task HandleAsync(ProductCountUpdated @event)
    {
        Console.WriteLine($"Item: {@event.Item}, Count: {@event.Count}");
        return Task.CompletedTask;
    }
}
