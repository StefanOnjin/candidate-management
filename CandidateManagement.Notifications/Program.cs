using CandidateManagement.Notifications.Hubs;
using CandidateManagement.Notifications.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSignalR();
builder.Services.AddHostedService<RabbitMqActivityEventConsumer>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("ReactClient");

app.MapHub<ActivityHub>("/hubs/activity");

app.MapGet("/", () => "CandidateManagement.Notifications is running.");

app.Run();
