using Microsoft.AspNetCore.Mvc;
using Pinecone.EventPublisher;

namespace Sample;

public class EventController : ControllerBase
{
    private readonly IPublisher<ProductCountUpdated> _productionCountUpdatedPublisher;

    public EventController(IPublisher<ProductCountUpdated> productionCountUpdatedPublisher)
    {
        _productionCountUpdatedPublisher = productionCountUpdatedPublisher;
    }

    [HttpGet("counts/{item}/{count}")]
    public async Task<IActionResult> PostProductionCountUpdatedAsync(string item, int count)
    {
        var @event = new ProductCountUpdated
        {
            Item = item,
            Count = count
        };
        await _productionCountUpdatedPublisher.PublishAsync(@event);

        return Ok();
    }
}
