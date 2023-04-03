namespace Mientreno.Compartido.Peticiones;

/*
    Desactiva avisos de que las string son nulas después del constructor.
    El aviso no aplica porque las propiedades son inicializadas mediante
    reflection por el serializador JSON.
*/
#pragma warning disable CS8618

/// <summary>
/// Datos de registro de un usuario.
/// </summary>
public class RegistroInput
{
    public string Login { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Correo { get; set; }
    public string Password { get; set; }
    public bool EsEntrenador { get; set; } = false;
}

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
/// Parámetros de entrada para obtener un token de acceso a partir de un token de refresco.
/// </summary>
public class RefrescarInput
{
    public string TokenRefresco { get; set; }
}