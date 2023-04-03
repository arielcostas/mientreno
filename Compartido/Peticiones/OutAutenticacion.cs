namespace Mientreno.Compartido.Peticiones;

/*
    Desactiva avisos de que las string son nulas después del constructor.
    El aviso no aplica porque las propiedades son inicializadas mediante
    reflection por el serializador JSON.
*/
#pragma warning disable CS8618

/// <summary>
/// La salida de la operación de inicio de sesión. Si se requiere un segundo factor de autenticación, el código se
/// debe volver a enviar al servidor como <see cref="LoginInput.Identificador"/> y la contraseña TOTP como
/// <see cref="LoginInput.Credencial"/>
/// </summary>
public class LoginOutput
{
    public string? Codigo { get; set; }
    public bool RequiereDesafio { get; set; }

    public string? TokenAcceso { get; set; }
    public string? TokenRefresco { get; set; }
}