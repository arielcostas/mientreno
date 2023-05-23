using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Helpers.Queue;
using Mientreno.Server.Models;

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

		_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
		{
			Idioma = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
			NombreDestinatario = $"{usuario.Nombre} {usuario.Apellidos}",
			DireccionDestinatario = usuario.Email!,
			Plantila = Constantes.EmailBienvenida,
			Parametros = new[] { usuario.Nombre }
		});

		return result.Succeeded ?
			RedirectToPage("/Login", new { confirmed = true }) :
			RedirectToPage("/Error");
	}
}