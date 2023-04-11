using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Server;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Crypto;
using Mientreno.Server.Helpers.Mailing;
using Mientreno.Server.Helpers.Services;
using Mientreno.Server.Models;
using Mientreno.Server.Services;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

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

    options.AddPolicy(Constantes.PolicyEsAlumno, policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "Alumno" }));
    options.AddPolicy(Constantes.PolicyEsEntrenador, policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "Entrenador" }));

    options.DefaultPolicy = options.GetPolicy("ValidSessionKey")!;
});

builder.Services.AddScoped<IAuthorizationHandler, ValidSessionKeyAuthorizationHandler>();

builder.Services.AddScoped<AutenticacionService>();
builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddTransient<IPasswordHasher<Usuario>>(sp =>
{
    return new PasswordHasher<Usuario>();
});

builder.Services.AddSingleton<IMailSender>((sp) =>
{
    var logger = sp.GetRequiredService<ILogger<AzureCSMailSender>>();
    string connectionString =
        builder.Configuration.GetConnectionString("AzureCS") ??
        throw new Exception("Se debe especificar una connectionString para el envío de correo por Azure Communication Services");
    string emailFrom =
        builder.Configuration.GetValue<string>("EmailFrom")
        ?? throw new Exception("Se debe especificar un EmailFrom en la configuración.");

    return new AzureCSMailSender(logger, connectionString, emailFrom);
});

builder.Services.AddSingleton<Cartero>();

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

app.MapControllers();

var cartero = app.Services.GetRequiredService<Cartero>();

Thread carteroThread = new(new ThreadStart(cartero.Run));
carteroThread.Start();

app.Run();
