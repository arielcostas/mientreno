#region Imports

using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Mientreno.Compartido;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Queue;
using Mientreno.Server.Models;
using RabbitMQ.Client;
using Sentry.AspNetCore;

#endregion

var builder = WebApplication.CreateBuilder(args);
var devel = builder.Environment.IsDevelopment();

#region Sentry

if (!devel)
{
	builder.Services.AddSentry().AddSentryOptions(options =>
	{
		options.Dsn = builder.Configuration.GetConnectionString("Sentry") ?? string.Empty;
		options.Debug = true;
		options.TracesSampleRate = devel ? 1.0 : 0.5;
		options.Environment = builder.Environment.EnvironmentName;
	});
	builder.Logging.AddSentry();
}
#endregion

builder.Services.AddProblemDetails();
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
});

builder.Services.AddDbContext<ApplicationContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddIdentity<Usuario, IdentityRole>()
	.AddDefaultTokenProviders()
	.AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
	{
		options.Cookie.Name = "_sd";
		options.Cookie.SameSite = SameSiteMode.Strict;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.Cookie.HttpOnly = true;
		options.Cookie.IsEssential = true;

		options.SlidingExpiration = true;
		options.ExpireTimeSpan = TimeSpan.FromDays(30);
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

#endregion

var app = builder.Build();

app.UseExceptionHandler();
app.UseHsts();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

if (!devel)
{
	app.UseForwardedHeaders();
	app.UseSentryTracing();
}

app.UseRequestLocalization();
app.UseStaticFiles();
app.MapRazorPages();

app.Run();