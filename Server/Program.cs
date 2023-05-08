using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Server;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Crypto;
using Mientreno.Server.Helpers.Queue;
using Mientreno.Server.Services;
using RabbitMQ.Client;
using Sentry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var devel = builder.Environment.IsDevelopment();

builder.Services.AddProblemDetails();
builder.Services.AddSentry().AddSentryOptions(options =>
{
	options.Dsn = builder.Configuration.GetConnectionString("Sentry") ?? string.Empty;
	options.Debug = true;
	options.TracesSampleRate = devel ? 1.0 : 0.5;
	options.Environment = builder.Environment.EnvironmentName;
});

builder.Logging.AddSentry();

builder.Services
	.AddControllers(options =>
	{
		options.Filters.Add<HttpExceptionFilter>();
	})
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(SigningKeyHolder.GetToken());
		options.TokenValidationParameters.ValidateAudience = false;
		options.TokenValidationParameters.ValidateIssuer = false;
		options.TokenValidationParameters.ValidateLifetime = true;
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ValidSessionKey", policy => policy.Requirements.Add(new ValidSessionKeyRequirement()));

	options.AddPolicy(Constantes.PolicyEsAlumno, policy => policy.RequireClaim(ClaimTypes.Role, "Alumno"));
	options.AddPolicy(Constantes.PolicyEsEntrenador, policy => policy.RequireClaim(ClaimTypes.Role, "Entrenador"));

	options.DefaultPolicy = options.GetPolicy("ValidSessionKey")!;
});

builder.Services.AddScoped<IAuthorizationHandler, ValidSessionKeyAuthorizationHandler>();

builder.Services.AddScoped<AutenticacionService>();
builder.Services.AddSingleton<TokenGenerator>();

// RabbitMQ
var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ Connection String not set");

builder.Services.AddSingleton(_ => new ConnectionFactory
{
	Uri = new Uri(rabbitConnectionString),
}.CreateConnection());

builder.Services.AddSingleton<IQueueProvider>(sp =>
{
	var connection = sp.GetRequiredService<IConnection>();
	return new RabbitQueueProvider(connection);
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseHsts();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(options =>
{
	options.WithOrigins(builder.Configuration.GetSection("Cors").Get<string[]>() ?? Array.Empty<string>());

	options.AllowAnyHeader();
	options.AllowAnyMethod();
});

app.UseSentryTracing();

app.MapControllers();
app.Run();
