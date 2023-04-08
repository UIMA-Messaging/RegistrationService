using ChannelService.Repository;
using ChannelService.Repository.Connection;
using System.Text.Json.Serialization;
using RegistrationApi.Errors;
using RegistrationApi.Services.Register;
using RegistrationApi.Contracts;
using RegistrationApi.EventBus.RabbitMQ.Connection;
using RegistrationApi.EventBus.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Serialization
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Controllers
builder.Services.AddControllers();

// Repositories
builder.Services.AddSingleton(_ => new UserRepository(new ConnectionFactory(builder.Configuration["ConnectionStrings:Users"])));

// RabbitMQ
var rabbitMQConnection = new RabbitMQConnection("localhost").TryConnect();

// RabbitMQ - Publishers
builder.Services.AddSingleton<IRabbitMQPublisher<RegisteredUser>>(_ => new RabbitMQPublisher<RegisteredUser>(rabbitMQConnection, "registrations"));

// Services
builder.Services.AddSingleton<IRegistrationService>(i => new RegistrationService(i.GetRequiredService<UserRepository>(), i.GetRequiredService<IRabbitMQPublisher<RegisteredUser>>()));

var app = builder.Build();

// Singleton instantiation
app.Services.GetService<IRegistrationService>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<HttpExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
