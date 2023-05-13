using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using Mientreno.Compartido.Peticiones;
using Mientreno.Compartido.Recursos;

namespace WebApp.Pages;

public partial class Login
{
	private readonly LoginModel _model = new();
	private string _customError = string.Empty;

	private async Task DoSignUp()
	{
		var resp = await Http.PostAsJsonAsync("Autenticacion/Registrar", new LoginInput()
		{
			Identificador = _model.Correo,
			Credencial = _model.Password,
		});

		if (resp.IsSuccessStatusCode)
		{
			// TODO: Do something
			NavigationManager.NavigateTo("/");
			return;
		}

		_customError = resp.StatusCode == HttpStatusCode.Unauthorized
			? AppStrings.errorInvalidCredentials
			: AppStrings.errorUnexpected;
	}
}

public class LoginModel
{
	[Required] [EmailAddress] public string Correo { get; set; } = string.Empty;

	[Required]
	public string Password { get; set; } = string.Empty;
}