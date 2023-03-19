using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Helpers;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.Audience = "Server";

        // Custom validation of the token's `nonce` claim against the database, to check if it was invalidated.
        options.Events = new()
        {
            OnTokenValidated = async context =>
            {
                var token = context.SecurityToken as JwtSecurityToken;
                if (token == null)
                {
                    context.Fail("Token was not a JWT");
                    return;
                }

                var sessid = token.Claims.First(c => c.Type == "nonce").Value;
                var service = context.HttpContext.RequestServices.GetRequiredService<AutenticacionService>();
                var userId = token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                if (await service.IsSessionValid(sessid, userId))
                {
                    context.Fail("Nonce was invalidated");
                }
            }
        };
    });

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