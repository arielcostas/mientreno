using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Connectors.Queue;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Pages;

public class ConfirmModel : PageModel
{
	private readonly UserManager<Usuario> _userManager;
	private readonly IQueueProvider _queueProvider;

	public ConfirmModel(UserManager<Usuario> userManager, IQueueProvider queueProvider)
	{
		_userManager = userManager;
		_queueProvider = queueProvider;
	}

	[FromQuery] public required string Id { get; set; }

	[FromQuery] public required string Token { get; set; }

	public async Task<IActionResult> OnGet()
	{
		var usuario = await _userManager.FindByIdAsync(Id);

		if (usuario == null)
		{
			return RedirectToPage("/Index");
		}

		var result = await _userManager.ConfirmEmailAsync(usuario, Token);

		var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
		var culture = rqf?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;
		
		var roles = await _userManager.GetRolesAsync(usuario);

		_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
		{
			Idioma = culture.TwoLetterISOLanguageName,
			NombreDestinatario = $"{usuario.Nombre} {usuario.Apellidos}",
			DireccionDestinatario = usuario.Email!,
			Plantila = roles.Contains(Entrenador.RoleName)
				? Constantes.EmailBienvenida
				: Constantes.EmailBienvenidaAlumno,
			Parametros = new[] { usuario.Nombre }
		});
		
		_queueProvider.Enqueue(Constantes.ColaGenerarFotoPerfil, new NuevaFoto()
		{
			Nombre = usuario.Nombre,
			Apellidos = usuario.Apellidos,
			IdUsuario = usuario.Id
		});

		return result.Succeeded ?
			Page() :
			RedirectToPage("/Error");
	}
}