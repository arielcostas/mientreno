using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service.Queue;

namespace Mientreno.Server.Pages;

public class ContactModel : PageModel
{
	private readonly IQueueProvider _queueProvider;
	
	public ContactModel(IQueueProvider queueProvider)
	{
		_queueProvider = queueProvider;
		Form = new ContactForm();
	}
	
	[BindProperty] public ContactForm Form { get; set; }
	public bool ContactoEnviado { get; set; } = false;

	public void OnGet()
	{
		Page();
	}

	public IActionResult OnPost()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}
		
		var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
		var culture = rqf?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;

		_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
		{
			Idioma = "es",
			NombreDestinatario = "Equipo de MiEntreno",
			DireccionDestinatario = "hola@mientreno.app",
			Plantila = Constantes.FormContacto,
			Parametros = new[] { Form.Nombre, Form.Email, Form.Mensaje, culture.DisplayName },
			ResponderA = Form.Email
		});
		
		Form.Nombre = string.Empty;
		Form.Email = string.Empty;
		Form.Mensaje = string.Empty;
		
		ContactoEnviado = true;
		return Page();
	}
}

public class ContactForm
{
	[Required] public string Nombre { get; set; }

	[Required] [EmailAddress] public string Email { get; set; }

	[Required] public string Mensaje { get; set; }
}