namespace Mientreno.Mobile;

public static class SettingsKeys
{
    public const string Refresco = "REFRESCO"; 
}

#nullable enable
public static class Global
{
    // TODO: Cambiar esto a una variable dependiente del entorno (localhost en desarrollo, dominio en producción)
    public static string BaseUrl { get; set; } = "https://localhost:7227/";
    public static string? Token { get; set; }
}