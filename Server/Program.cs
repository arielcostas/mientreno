#region Imports

using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Mientreno.Compartido;
using Mientreno.QueueWorker;
using Mientreno.QueueWorker.Mailing;
using Mientreno.Server.Connectors.Queue;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Workers;
using RabbitMQ.Client;
using Stripe;

#endregion

var builder = WebApplication.CreateBuilder(args);
var devel = builder.Environment.IsDevelopment();

#region Stripe

StripeConfiguration.ApiKey = builder.Configuration["Stripe:Secret"];

#endregion

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

builder.Services.AddRequestLocalization(options =>
{
	var supportedCultures = new[]
	{
		new CultureInfo(Idiomas.Castellano),
		new CultureInfo(Idiomas.Gallego),
		new CultureInfo(Idiomas.Catalán),
		new CultureInfo(Idiomas.Euskera),
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

builder.Services.AddDbContextPool<ApplicationDatabaseContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddIdentity<Usuario, IdentityRole>(options => { options.SignIn.RequireConfirmedAccount = true; })
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

#region RabbitMQ

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

#endregion

builder.Services.AddSingleton<IMailSender>(sp =>
{
	var logger = sp.GetRequiredService<ILogger<IMailSender>>();

	var emailFrom = builder.Configuration.GetValue<string>("EmailFrom") ?? throw new Exception("EmailFrom not set");

	var secretKey = builder.Configuration["Scaleway:SecretKey"] ??
	                throw new Exception("ScalewayProjectId not set");
	var projectId = builder.Configuration["Scaleway:ProjectId"] ??
	                throw new Exception("ScalewayProjectId not set");

	return new ScalewayMailSender(logger, emailFrom, secretKey, projectId);
});

var runProfileGenerator = builder.Configuration.GetValue<bool?>("Workers:RunProfileGenerator") ?? true;
var runMailQueue = builder.Configuration.GetValue<bool?>("Workers:RunEmailSender") ?? true;

if (runProfileGenerator)
{
	builder.Services.AddHostedService<ProfilePhotoGeneratorWorker>();
}

if (runMailQueue)
{
	builder.Services.AddHostedService<MailQueueWorker>();
}

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

if (!devel)
{
	app.UseForwardedHeaders();
}

app.UseRequestLocalization();
app.UseStaticFiles();

app.UseFileServer(new FileServerOptions()
{
	RequestPath = "/Static",
	FileProvider = new PhysicalFileProvider(
		builder.Configuration["FileBase"] ?? throw new Exception("FileBase not set")
	),
	EnableDirectoryBrowsing = false,
});

app.MapRazorPages();
app.MapControllers();

app.Run();