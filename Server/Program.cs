using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Server;
using Mientreno.Server.Helpers;
using Mientreno.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMailSender>((sp) =>
{
    ILogger<AzureCSMailSender> logger = sp.GetService<ILoggerFactory>().CreateLogger<AzureCSMailSender>();
    var conn = builder.Configuration.GetConnectionString("AzureEmailCS");
    var from = builder.Configuration.GetValue<string>("EmailFrom");

    if (string.IsNullOrEmpty(conn))
    {
        throw new Exception("`EmailService` connection string cannot be null.");
    }

    if (string.IsNullOrEmpty(from))
    {
        throw new Exception("`EmailFrom` settings value cannot be null.");
    }

    return new AzureCSMailSender(logger, conn, from);
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

    options.AddPolicy("EsAlumno", policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "Alumno" }));
    options.AddPolicy("EsEntrenador", policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "Entrenador" }));

    options.DefaultPolicy = options.GetPolicy("ValidSessionKey")!;
});

builder.Services.AddScoped<IAuthorizationHandler, ValidSessionKeyAuthorizationHandler>();

builder.Services.AddScoped<AutenticacionService>();
builder.Services.AddSingleton<TokenGenerator>();

var app = builder.Build();

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

app.Run();