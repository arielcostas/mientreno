#region Imports

using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Mientreno.Compartido;
using Mientreno.Server;
using Mientreno.Server.Connectors.Queue;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Workers;
using RabbitMQ.Client;
using Stripe;

#endregion

var builder = WebApplication.CreateBuilder(args);
var devel = builder.Environment.IsDevelopment();
StripeConfiguration.ApiKey = builder.Configuration["Stripe:Secret"];

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddRazorPages();
builder.Services.AddDbContextPool<ApplicationDatabaseContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.SetupLocalisation();
builder.SetupAccessControl();
await builder.AddRabbitMq();

var runProfileGenerator = builder.Configuration.GetValue<bool?>("Workers:RunProfileGenerator") ?? true;

if (runProfileGenerator)
{
	builder.Services.AddHostedService<ProfilePhotoGeneratorWorker>();
}

builder.ConditionallyAddProfileGenerator();
builder.ConditionallyAddEmail();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseForwardedHeaders();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");

app.UseRequestLocalization();
app.UseStaticFiles();

app.UseFileServer(new FileServerOptions
{
	RequestPath = "/Static",
	FileProvider = new PhysicalFileProvider(
		builder.Configuration["FileBase"] ?? throw new Exception("FileBase not set")
	),
	EnableDirectoryBrowsing = devel,
});

app.MapRazorPages();
app.MapControllers();

app.Run();