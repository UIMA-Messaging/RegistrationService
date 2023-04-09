using RegistrationService.Repository;
using RegistrationService.Repository.Connection;
using System.Text.Json.Serialization;
using Bugsnag;
using RegistrationService.Exceptions;
using RegistrationService.Services.Registerations;
using RegistrationService.Contracts;
using RegistrationService.EventBus.RabbitMQ.Connection;
using RegistrationService.EventBus.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bugsnag
builder.Services.AddSingleton<IClient>(_ => new Client(builder.Configuration["Bugsnag:ApiKey"]));

// Serialization
builder.Services.AddControllers().AddJsonOptions(configs =>
{
    configs.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    configs.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
builder.Services.AddSingleton<IUserService>(s => new RegistrationService.Services.Registerations.UserService(s.GetRequiredService<UserRepository>(), s.GetRequiredService<IRabbitMQPublisher<RegisteredUser>>()));

var app = builder.Build();

// Singleton instantiation
app.Services.GetService<IUserService>();

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
