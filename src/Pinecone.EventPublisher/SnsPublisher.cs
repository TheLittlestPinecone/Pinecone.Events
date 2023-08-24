using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Pinecone.EventPublisher;

public class SnsPublisher<TEvent> : IPublisher<TEvent>
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly string _topicArn;
    private readonly string _eventType;

    public SnsPublisher(IAmazonSimpleNotificationService snsClient, string topicArn, string eventType)
    {
        _snsClient = snsClient;
        _topicArn = topicArn;
        _eventType = eventType;
    }

    public async Task PublishAsync(TEvent @event)
    {
        var message = JsonSerializer.Serialize(@event);
        var request = new PublishRequest(_topicArn, message)
        {
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                { "event_type", new MessageAttributeValue { DataType = "String", StringValue = _eventType } }
            }
        };
        Console.WriteLine($"Using event type: {_eventType}");
        var response = await _snsClient.PublishAsync(request);
        Console.WriteLine($"Got back status code: {response.HttpStatusCode}");
        Console.WriteLine($"Published event to SNS: {response.MessageId}");
    }
}

public interface IPublisherFactory
{
    IPublisher<TEvent> Create<TEvent>(string eventType);
}

public class SnsPublisherFactory : IPublisherFactory
{
    private readonly IAmazonSimpleNotificationService _snsClient;
    private readonly string _topicArn;

    public SnsPublisherFactory(IAmazonSimpleNotificationService snsClient, IOptions<SnsConfig> config)
    {
        _snsClient = snsClient;
        _topicArn = config.Value.TopicArn;
    }

    public IPublisher<TEvent> Create<TEvent>(string eventType)
    {
        return new SnsPublisher<TEvent>(_snsClient, _topicArn, eventType);
    }
}

public class SnsConfig
{
    public string TopicArn { get; set; }
}

public class PublisherBuilder<TPublisherFactory> where TPublisherFactory : IPublisherFactory
{
    private readonly IServiceCollection _services;

    public PublisherBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public PublisherBuilder<TPublisherFactory> Register<TEvent>(string eventType)
    {
        _services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<TPublisherFactory>().Create<TEvent>(eventType));
        return this;
    }
}

public static class ServiceCollectionExtensions
{
    public static PublisherBuilder<TPublisherFactory> AddEventPublishers<TPublisherFactory>(this IServiceCollection services) where TPublisherFactory : IPublisherFactory
    {
        return new PublisherBuilder<TPublisherFactory>(services);
    }

    public static PublisherBuilder<SnsPublisherFactory> AddSnsEventPublishers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.Configure<SnsConfig>(configuration);
        services.AddSingleton<SnsPublisherFactory>();
        return new PublisherBuilder<SnsPublisherFactory>(services);
    }
}
