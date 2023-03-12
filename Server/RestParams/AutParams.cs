namespace Server.RestParams;

/// <summary>
/// Parámetros de entrada para iniciar sesión.
/// </summary>
public class LoginInput
{
    /// <summary>
    /// Identifica al usuario. Puede ser un nombre de usuario o un correo electrónico para el login; o un
    /// <see cref="LoginOutput.Codigo"/> para el segundo factor de autenticación.
    /// </summary>
    public string Identificador { get; set; }

    /// <summary>
    /// La credencial para identificarse. Puede ser una contraseña, si el Identificador es un nombre de usuario o
    /// correo electrónico; o un código TOTP, si el Identificador es un código de inicio de sesión.
    /// </summary>
    public string Credencial { get; set; }
}

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