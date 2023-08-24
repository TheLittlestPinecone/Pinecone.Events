using System;

namespace Pinecone.Events;

public interface IEventSource
{
    string Id { get; }
    string Name { get; }
}

public class EventSource : IEventSource
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public interface IParentEvent
{
    string Id { get; }
    string Type { get; }
}

public class ParentEvent : IParentEvent
{
    public string Id { get; set; } = default!;
    public string Type { get; set; } = default!;
}

public interface IEventData
{
}

public class SubscriberEvent : IParentEvent
{
    public string Id { get; init; } = default!;
    public string Type { get; init; } = default!;
    public DateTime Timestamp { get; init; }
    public IEventSource Source { get; init; } = default!;
    public IParentEvent? ParentEvent { get; init; }
    public string Data { get; init; } = default!;
}

public class SubscriberEvent<TEventData> : IParentEvent where TEventData : IEventData
{
    public string Id { get; init; } = default!;
    public string Type { get; init; } = default!;
    public DateTime Timestamp { get; init; }
    public IEventSource Source { get; init; } = default!;
    public IParentEvent? ParentEvent { get; init; }
    public TEventData Data { get; init; } = default!;
}

public class PublisherEvent<TEventData> where TEventData : IEventData
{
    public DateTime? Timestamp { get; set; }
    public EventSource? Source { get; set; }
    public ParentEvent? ParentEvent { get; set; }
    public TEventData Data { get; set; } = default!;
}
