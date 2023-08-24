using System;

namespace Pinecone.Events;

public interface IEventSource
{
    string System { get; }
    public ISubscriberEvent? ParentEvent { get; }
}

public class EventSource : IEventSource
{
    public string System { get; init; } = default!;
    public ISubscriberEvent? ParentEvent { get; init; }
}

public interface IEvent
{
    string Type { get; }
    DateTime Timestamp { get; }
}

public interface ISubscriberEvent : IEvent
{
    string Id { get; }
}

public interface IPublisherEvent : IEvent
{
    public ISubscriberEvent? ParentEvent { get; }
}

public interface IEventData
{
}

public class SubscriberEvent : ISubscriberEvent
{
    public string Id { get; init; } = default!;
    public string Type { get; init; } = default!;
    public DateTime Timestamp { get; init; }
    public IEventSource Source { get; init; } = default!;
    public string Data { get; init; } = default!;
}

public class SubscriberEvent<TEventData> : ISubscriberEvent where TEventData : IEventData
{
    public string Id { get; init; } = default!;
    public string Type { get; init; } = default!;
    public DateTime Timestamp { get; init; }
    public IEventSource Source { get; init; } = default!;
    public TEventData Data { get; init; } = default!;
}

public class PublisherEvent<TEventData> : IPublisherEvent where TEventData : IEventData
{
    public string Type { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public ISubscriberEvent? ParentEvent { get; set; }
    public TEventData Data { get; set; } = default!;
}
