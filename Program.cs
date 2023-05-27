using RegistrationService.Repository;
using RegistrationService.Repository.Connection;
using System.Text.Json.Serialization;
using RegistrationService.Contracts;
using RegistrationService.Services;
using Bugsnag;
using RegistrationService.Middlewares;
using RegistrationService.RabbitMQ;
using RegistrationService.RabbitMQ.Connection;

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
builder.Services.AddSingleton<IUserRepository>(_ => new UserRepository(new ConnectionFactory(builder.Configuration["ConnectionStrings:Users"])));

//// RabbitMQ
builder.Services.AddSingleton<IRabbitMQConnection>(_ => new RabbitMQConnection(builder.Configuration["RabbitMQ:Host"], builder.Configuration["RabbitMQ:Username"], builder.Configuration["RabbitMQ:Password"]));
builder.Services.AddSingleton<IRabbitMQPublisher<RegisteredUser>>(s => new RabbitMQPublisher<RegisteredUser>(s.GetRequiredService<IRabbitMQConnection>(), builder.Configuration["RabbitMQ:Exchange"]));
builder.Services.AddSingleton<IRabbitMQPublisher<ExchangeKeys>>(s => new RabbitMQPublisher<ExchangeKeys>(s.GetRequiredService<IRabbitMQConnection>(), builder.Configuration["RabbitMQ:Exchange"]));

// Services
builder.Services.AddSingleton(s => new UserService(s.GetRequiredService<IUserRepository>(), builder.Configuration["Jabber:Host"], s.GetRequiredService<IRabbitMQPublisher<RegisteredUser>>(), s.GetRequiredService<IRabbitMQPublisher<ExchangeKeys>>()));

var app = builder.Build();

// Singleton instantiation
app.Services.GetService<UserService>();

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
