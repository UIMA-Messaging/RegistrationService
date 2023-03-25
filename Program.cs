using ChannelService.Repository;
using ChannelService.Repository.Connection;
using System.Text.Json.Serialization;
using RegistrationApi.Errors;
using RegistrationApi.Services.Register;
using RegistrationApi.EventBus;
using RegistrationApi.Contracts;

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
builder.Services.AddSingleton(_ => new RabbitMQHelper<RegisteredUser>("localhost", "registrationQueue", "registrations"));

// Services
builder.Services.AddTransient<IRegistrationService>(i => new RegistrationService(i.GetRequiredService<UserRepository>(), i.GetRequiredService<RabbitMQHelper<RegisteredUser>>()));

var app = builder.Build();

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
