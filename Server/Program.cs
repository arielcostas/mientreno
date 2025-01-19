using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Mientreno.Server;
using Mientreno.Server.Data;
using Mientreno.Server.Workers;

var builder = WebApplication.CreateBuilder(args);
var devel = builder.Environment.IsDevelopment();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddRazorPages();
builder.Services.AddDbContextPool<ApplicationDatabaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Database");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new Exception("Connection string Database not set");
    }

    options.UseMySQL(connectionString);
});

builder.SetupLocalisation();
builder.SetupAccessControl();
await builder.AddRabbitMq();

var runProfileGenerator = builder.Configuration.GetValue<bool?>("Workers:ProfileImageGenerator") ?? true;

if (runProfileGenerator)
{
    builder.Services.AddHostedService<ProfilePhotoGeneratorWorker>();
}

builder.ConditionallyAddProfileGenerator();
builder.ConditionallyAddEmail();

var app = builder.Build();

app.UseExceptionHandler();

if (devel)
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