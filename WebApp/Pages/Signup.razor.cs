using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using Mientreno.Compartido.Peticiones;
using Mientreno.Compartido.Recursos;

namespace WebApp.Pages;

public partial class Signup
{
	private readonly SignupModel _model = new();
	private string _customError = string.Empty;
	private bool _signupSuccess = false;

	private async Task DoSignUp()
	{
		var resp = await Http.PostAsJsonAsync("Autenticacion/Registrar", new RegistroInput
		{
			Nombre = _model.Nombre,
			Apellido = _model.Apellidos,
			Correo = _model.Correo,
			Contraseña = _model.Password,
		});

		if (resp.IsSuccessStatusCode)
		{
			_signupSuccess = true;
			return;
		}

		_customError = resp.StatusCode == HttpStatusCode.Conflict
			? AppStrings.usernameOrEmailAlreadyInUse
			: AppStrings.errorUnexpected;
	}
}

public class SignupModel
{
	[Required] public string Nombre { get; set; } = string.Empty;

	[Required] public string Apellidos { get; set; } = string.Empty;
	[Required] [EmailAddress] public string Correo { get; set; } = string.Empty;

	[Required]
	[RegularExpression("^[A-Za-z0-9\\D]{8,}", ErrorMessageResourceType = typeof(AppStrings),
		ErrorMessageResourceName = nameof(AppStrings.passwordTooWeak))]
	public string Password { get; set; } = string.Empty;
}