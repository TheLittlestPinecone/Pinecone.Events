using Pinecone.EventSubscriber;

namespace Sample;

public class ProductionItemCompleted
{
}

public class ProductionItemCompletedSubscriber : ISubscriber<ProductionItemCompleted>
{
    public Task HandleAsync(ProductionItemCompleted @event)
    {
        Console.WriteLine("ProductionItemCompleted");
        return Task.CompletedTask;
    }
}
