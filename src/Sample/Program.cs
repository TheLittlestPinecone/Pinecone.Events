using Amazon.SimpleNotificationService;
using Pinecone.EventPublisher;
using Pinecone.EventSubscriber;
using Sample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSnsEventPublishers(builder.Configuration.GetSection("Sns"))
    .Register<ProductionItemCompleted>("ProductionItemCompleted")
    .Register<ProductCountUpdated>("ProductCountUpdated");

builder.Services.AddSqsEventSubscribers()
    .UseHandler<ProductionItemCompletedSubscriber>("ProductionItemCompleted")
    .UseHandler<ProductionCountUpdatedSubscriber>("ProductCountUpdated");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
