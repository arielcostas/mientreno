using QueueWorker;
using QueueWorker.Mailing;
using RabbitMQ.Client;

var hb = Host.CreateDefaultBuilder(args);

hb.ConfigureLogging((context, loggingBuilder) =>
{
	loggingBuilder.AddConfiguration(context.Configuration);
	loggingBuilder.AddConsole();

		loggingBuilder.AddSentry(o =>
		{
			o.Dsn = context.Configuration.GetConnectionString("Sentry") ?? string.Empty;
			o.Debug = true;
			o.EnableTracing = !context.HostingEnvironment.IsDevelopment();
			o.TracesSampleRate = 1.0;
			o.Environment = context.HostingEnvironment.EnvironmentName;
		});
	
});

hb.ConfigureServices((context, services) =>
{
	#region RabbitMQ

	var rabbitConnectionString = context.Configuration.GetConnectionString("RabbitMQ") ??
	                             throw new Exception("RabbitMQ Connection String not set");

	services.AddSingleton(_ => new ConnectionFactory
	{
		Uri = new Uri(rabbitConnectionString),
	}.CreateConnection());

	#endregion

	#region AzureCS

	services.AddSingleton<IMailSender>(sp =>
	{
		var logger = sp.GetRequiredService<ILogger<AzureCSMailSender>>();

		var connectionString = context.Configuration.GetConnectionString("AzureCS") ??
		                       throw new Exception("AzureCS Connection String not set");

		var emailFrom = context.Configuration.GetValue<string>("EmailFrom") ?? throw new Exception("EmailFrom not set");

		return new AzureCSMailSender(logger, connectionString, emailFrom);
	});

	#endregion

	services.AddHostedService<MailQueueWorker>();
	//services.AddHostedService<UserDeletionQueueWorker>();
});

var host = hb.Build();

host.Run();