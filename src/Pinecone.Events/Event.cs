using System;
using System.Collections.Generic;

namespace Pinecone.Events;

public abstract class EventAttribute
{
    public EventAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; } = string.Empty;
    public abstract Type Type { get; }
}

public class EventAttribute<TValue> : EventAttribute
{
    public EventAttribute(string name) : base(name)
    {
    }

    public override Type Type => typeof(TValue);
}

public static class Attributes
{
    public static readonly EventAttribute<string> EventType = new("event_type");
}

public class AttributeCollection
{
    private readonly IDictionary<EventAttribute, object> _attributes = new Dictionary<EventAttribute, object>();

    private AttributeCollection(IDictionary<EventAttribute, object> attributes)
    {
        _attributes = attributes;
    }

    public TValue Get<TValue>(EventAttribute<TValue> key)
    {
        return (TValue)_attributes[key];
    }

    public static AttributeCollectionBuilder Builder()
    {
        return new();
    }

    public class AttributeCollectionBuilder
    {
        private readonly IDictionary<EventAttribute, object> _attributes = new Dictionary<EventAttribute, object>();

        public AttributeCollectionBuilder With<TValue>(EventAttribute<TValue> key, TValue value)
        {
            _attributes[key] = value;
            return this;
        }

        public AttributeCollection Build()
        {
            return new(_attributes);
        }
    }
}

public class Event<TEvent>
{
    public TEvent EventData { get; set; } = default!;
    public AttributeCollection Attributes { get; set; } = default!;
}

public class Sample
{
    public void Main()
    {
        var attributes = AttributeCollection.Builder()
            .With(Attributes.EventType, "product_count_updated")
            .Build();
        var @event = new Event<(int, int)>
        {
            EventData = (1, 3),
            Attributes = AttributeCollection.Builder()
                .With(Attributes.EventType, "product_count_updated")
                .Build()
        };
    }
}
