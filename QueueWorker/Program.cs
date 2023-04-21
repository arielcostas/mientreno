using QueueWorker;
using RabbitMQ.Client;

IHostBuilder hb = Host.CreateDefaultBuilder(args);

hb.ConfigureServices((context, services) =>
{
    var rabbitConnectionString = context.Configuration.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ Connection String not set");

    services.AddSingleton((sp) =>
    {
        return new ConnectionFactory()
        {
            Uri = new Uri(rabbitConnectionString),
        }.CreateConnection();
    });

    services.AddHostedService<MailQueueWorker>();
    //services.AddHostedService<UserDeletionQueueWorker>();
});

var host = hb.Build();

host.Run();
