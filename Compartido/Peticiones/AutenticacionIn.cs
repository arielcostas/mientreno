namespace Mientreno.Compartido.Peticiones;

/// <summary>
/// Datos de registro de un usuario.
/// </summary>
public class RegistroInput
{
	/// <summary>
	/// Identificador público del usuario
	/// </summary>
	public string Login { get; set; }
	/// <summary>
	/// Nombre real
	/// </summary>
	public string Nombre { get; set; }
	/// <summary>
	/// Apellido(s) real(es)
	/// </summary>
	public string Apellido { get; set; }
	/// <summary>
	/// Dirección de correo electrónico para notificaciones
	/// </summary>
	public string Correo { get; set; }
	/// <summary>
	/// Contraseña del usuario
	/// </summary>
	public string Contraseña { get; set; }
	/// <summary>
	/// Si el usuario accederá a la sección de entrenadores. Si es `true`, es Entrenador. Si es `false` es Alumno.
	/// </summary>
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

/// <summary>
/// 
/// </summary>
public class ConfirmarInput
{
	public string DireccionEmail { get; set; }
	public string CodigoVerificacion { get; set; }
}