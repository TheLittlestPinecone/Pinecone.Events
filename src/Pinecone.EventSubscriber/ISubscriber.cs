using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Pinecone.EventSubscriber;

public interface ISubscriber
{
    Task HandleAsync(string @event);
}

public interface ISubscriber<TEvent> : ISubscriber
{
    Task ISubscriber.HandleAsync(string @event)
    {
        var typedEvent = JsonSerializer.Deserialize<TEvent>(@event) ?? throw new InvalidOperationException("Failed to deserialize event");
        return HandleAsync(typedEvent);
    }

    Task HandleAsync(TEvent @event);
}

public abstract class SqsEventFunctionHandler
{
    private readonly ISubscriberRegistry _subscribers;

    protected SqsEventFunctionHandler(ISubscriberRegistry subscribers)
    {
        _subscribers = subscribers;
    }

    public Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            var eventType = record.MessageAttributes["event_type"].StringValue;
            var @event = record.Body;
            _subscribers.GetSubscriber(eventType).HandleAsync(@event);
        }

        return Task.CompletedTask;
    }
}

public interface ISubscriberRegistry
{
    ISubscriber GetSubscriber(string eventType);
}

public class SubscriberRegistry : ISubscriberRegistry
{
    private readonly IDictionary<string, ISubscriber> _subscribers;

    public SubscriberRegistry(IDictionary<string, ISubscriber> subscribers)
    {
        _subscribers = subscribers;
    }

    public ISubscriber GetSubscriber(string eventType)
    {
        return _subscribers[eventType];
    }
}

public interface ISubscriberBuilder
{
    ISubscriberBuilder UseHandler<TSubscriber>(string eventType) where TSubscriber : class, ISubscriber;
}

public class SubscriberBuilder : ISubscriberBuilder
{
    private readonly IServiceCollection _services;
    private readonly IDictionary<string, Type> _subscribers = new Dictionary<string, Type>();


    public SubscriberBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public ISubscriberBuilder UseHandler<TSubscriber>(string eventType) where TSubscriber : class, ISubscriber
    {
        _services.AddTransient<TSubscriber>();
        _subscribers.Add(eventType, typeof(TSubscriber));
        return this;
    }

    public ISubscriberRegistry Build(IServiceProvider serviceProvider)
    {
        var subscribers = new Dictionary<string, ISubscriber>();
        foreach (var (eventType, subscriberType) in _subscribers)
        {
            var subscriber = (ISubscriber)serviceProvider.GetRequiredService(subscriberType);
            subscribers.Add(eventType, subscriber);
        }

        return new SubscriberRegistry(subscribers);
    }
}

public static class ServiceCollectionExtensions
{
    public static ISubscriberBuilder AddSqsEventSubscribers(this IServiceCollection services)
    {
        var subscriberBuilder = new SubscriberBuilder(services);
        services.AddSingleton(serviceProvider => subscriberBuilder.Build(serviceProvider));
        return subscriberBuilder;
    }
}
