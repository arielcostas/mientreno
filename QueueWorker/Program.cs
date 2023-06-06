using Mientreno.QueueWorker;
using Mientreno.QueueWorker.Mailing;
using RabbitMQ.Client;

var hb = Host.CreateDefaultBuilder(args);

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
		var logger = sp.GetRequiredService<ILogger<IMailSender>>();

		var emailFrom = context.Configuration.GetValue<string>("EmailFrom") ?? throw new Exception("EmailFrom not set");

		var secretKey = context.Configuration["Scaleway:SecretKey"] ??
		                throw new Exception("ScalewayProjectId not set");
		var projectId = context.Configuration["Scaleway:ProjectId"] ??
		                throw new Exception("ScalewayProjectId not set");

		return new ScalewayMailSender(logger, emailFrom, secretKey, projectId);
	});

	#endregion
	
	services.AddSingleton(context.Configuration);

	services.AddHostedService<MailQueueWorker>();
	services.AddHostedService<ProfilePhotoGeneratorWorker>();
});

var host = hb.Build();

host.Run();