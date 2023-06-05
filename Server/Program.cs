#region Imports

using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Mientreno.Compartido;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service.Queue;
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
});

builder.Services.AddDbContextPool<ApplicationDatabaseContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddIdentity<Usuario, IdentityRole>()
	.AddDefaultTokenProviders()
	.AddEntityFrameworkStores<ApplicationDatabaseContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Login";
	options.LogoutPath = "/Logout";
	options.AccessDeniedPath = "/Error"; // TODO: Create Error Page
	options.Cookie.Name = "_met";
	options.Cookie.HttpOnly = true;
	options.Cookie.SameSite = SameSiteMode.Strict;
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

#endregion

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