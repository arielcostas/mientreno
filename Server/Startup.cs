using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Mientreno.Compartido;
using Mientreno.QueueWorker;
using Mientreno.QueueWorker.Mailing;
using Mientreno.Server.Connectors.Queue;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Workers;
using RabbitMQ.Client;

namespace Mientreno.Server;

public static class Startup
{
	public static void ConditionallyAddEmail(this WebApplicationBuilder builder)
	{
		var runMailQueue = builder.Configuration.GetValue<bool?>("Workers:RunEmailSender") ?? true;
		if (!runMailQueue) return;

		builder.Services.AddSingleton<IMailSender>(sp =>
		{
			var logger = sp.GetRequiredService<ILogger<IMailSender>>();

			var emailFrom = builder.Configuration.GetValue<string>("EmailFrom") ??
			                throw new Exception("EmailFrom not set");

			var secretKey = builder.Configuration["Scaleway:SecretKey"] ??
			                throw new Exception("ScalewayProjectId not set");
			var projectId = builder.Configuration["Scaleway:ProjectId"] ??
			                throw new Exception("ScalewayProjectId not set");

			return new ScalewayMailSender(logger, emailFrom, secretKey, projectId);
		});

		builder.Services.AddHostedService<MailQueueWorker>();
	}

	public static void AddRabbitMq(this WebApplicationBuilder builder)
	{
		var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ??
		                             throw new Exception("RabbitMQ Connection String not set");

		builder.Services.AddSingleton(_ => new ConnectionFactory
		{
			Uri = new Uri(rabbitConnectionString),
		}.CreateConnection());

		builder.Services.AddSingleton<IQueueProvider>(sp =>
		{
			var connection = sp.GetRequiredService<IConnection>();
			return new RabbitQueueProvider(connection);
		});

		builder.Services.AddSingleton(
			typeof(IQueueConsumer<>),
			typeof(RabbitQueueConsumer<>)
		);
	}

	public static void SetupLocalisation(this WebApplicationBuilder builder)
	{
		builder.Services.AddRequestLocalization(options =>
		{
			var supportedCultures = new[]
			{
				new CultureInfo(Idiomas.Castellano),
				new CultureInfo(Idiomas.Gallego),
				new CultureInfo(Idiomas.Catalán),
				new CultureInfo(Idiomas.Euskera),
				new CultureInfo(Idiomas.Inglés)
			};

			options.DefaultRequestCulture = new RequestCulture(Idiomas.Castellano);
			options.SupportedCultures = supportedCultures;
			options.SupportedUICultures = supportedCultures;

			options.RequestCultureProviders = new List<IRequestCultureProvider>
			{
				new CookieRequestCultureProvider
				{
					CookieName = "_lang",
					Options = options
				},
				new AcceptLanguageHeaderRequestCultureProvider()
			};
		});
	}

	public static void SetupAccessControl(this WebApplicationBuilder builder)
	{
		builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
			})
			.AddDefaultTokenProviders()
			.AddEntityFrameworkStores<ApplicationDatabaseContext>();

		builder.Services.ConfigureApplicationCookie(options =>
		{
			options.LoginPath = "/Login";
			options.LogoutPath = "/Logout";
			options.AccessDeniedPath = "/Error"; // TODO: Create Error Page
			options.Cookie.Name = "_met";
			options.Cookie.HttpOnly = true;
			options.Cookie.SameSite = SameSiteMode.Lax;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			options.ExpireTimeSpan = TimeSpan.FromDays(7);
			options.SlidingExpiration = true;
		});

		builder.Services.AddAuthorization();
	}

	public static void ConditionallyAddProfileGenerator(this WebApplicationBuilder builder)
	{
		var runProfileGenerator = builder.Configuration.GetValue<bool?>("Workers:RunProfileGenerator") ?? true;

		if (runProfileGenerator)
		{
			builder.Services.AddHostedService<ProfilePhotoGeneratorWorker>();
		}
	}
}