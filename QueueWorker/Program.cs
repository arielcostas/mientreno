using QueueWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<MailQueueWorker>();
        services.AddHostedService<UserDeletionQueueWorker>();
    })
    .Build();

host.Run();
