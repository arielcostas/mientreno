using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Business;
using Mientreno.Server.Connectors.Queue;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Pages;

public class ContactModel : PageModel
{
	private readonly IQueueProvider _queueProvider;
	private UserManager<Usuario> _userManager;
	private ApplicationDatabaseContext _databaseContext;

	public ContactModel(IQueueProvider queueProvider, UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext)
	{
		_queueProvider = queueProvider;
		_userManager = userManager;
		_databaseContext = databaseContext;
		Form = new ContactForm();
	}
	
	[BindProperty] public ContactForm Form { get; set; }
	public bool ContactoEnviado { get; set; } = false;
	public bool LoggedInPrefill { get; set; } = false;

	public void OnGet()
	{
		var user = _userManager.GetUserAsync(User).Result;
		
		if (user != null)
		{
			Form.Nombre = user.Nombre;
			Form.Email = user.Email!;
			LoggedInPrefill = true;
		}

		Page();
	}

	public IActionResult OnPost()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var nombre = Form.Nombre;
		var email = Form.Email;

		var user = _userManager.GetUserAsync(User).Result;
		var priority = false;

		if (user != null)
		{
			nombre = user.Nombre;
			email = user.Email!;

			if (user is Entrenador)
			{
				var plan = _databaseContext.Entrenadores
					.Include(e => e.Suscripcion)
					.Where(e => e.Id == user.Id)
					.Select(e => e.Suscripcion.Plan)
					.FirstOrDefault();
				priority = SubscriptionRestrictions.PrioritySupport(plan);
			}
		}
		
		var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
		var culture = rqf?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;

		_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
		{
			Idioma = Idiomas.Castellano,
			NombreDestinatario = "Equipo de MiEntreno",
			DireccionDestinatario = "hola@mientreno.app",
			Plantila = Constantes.FormContacto,
			Parametros = new[] { nombre, email, Form.Mensaje, culture.DisplayName, priority.ToString() },
			ResponderA = email
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
	[Required] public string Nombre { get; set; } = string.Empty;

	[Required] [EmailAddress] public string Email { get; set; } = string.Empty;

	[Required] public string Mensaje { get; set; } = string.Empty;
}