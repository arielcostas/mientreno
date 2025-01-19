using System.ComponentModel.DataAnnotations;
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

public class ForgetModel : PageModel
{
	private readonly ILogger<ForgetModel> _logger;
	private readonly SignInManager<Usuario> _signInManager;
	private readonly UserManager<Usuario> _userManager;
	private readonly IQueueProvider _queueProvider;

	public ForgetModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, ILogger<ForgetModel> logger, IQueueProvider queueProvider)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_logger = logger;
		_queueProvider = queueProvider;
	}

	[BindProperty] public ForgetForm Form { get; set; } = new();

	public bool MensajeEnviado { get; set; } = false;
	
	public IActionResult OnGetAsync()
	{
		return Page();
	}
	
	public async Task<IActionResult> OnPost(bool rememberMe, string? returnUrl = null)
	{
		if (!ModelState.IsValid) return Page();
		
		var usuario = _userManager.GetUserAsync(User).Result;
		if (usuario is not null)
		{
			return Redirect("/");
		}

		var user = await _userManager.FindByEmailAsync(Form.Email);
		if (user == null)
		{
			_logger.LogWarning("ForgotPassword: User not found for email {Email}", Form.Email);
			MensajeEnviado = true;
			return Page();
		}
		
		var code = await _userManager.GeneratePasswordResetTokenAsync(user);
		var callbackUrl = Url.Page(
			"/PasswordReset",
			pageHandler: null,
			values: new { code, Form.Email },
			protocol: Request.Scheme);

		var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
		var culture = rqf?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;
		_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
		{
			Idioma = culture.TwoLetterISOLanguageName,
			NombreDestinatario = $"{user.Nombre} {user.Apellidos}",
			DireccionDestinatario = user.Email!,
			Plantila = Constantes.EmailOlvideContraseña,
			Parametros = new[] { user.Nombre, callbackUrl! }
		});

		_logger.LogInformation("ForgotPassword: Sending email to {Email}", Form.Email);
		
		MensajeEnviado = true;
		return Page();
	}

}

public class ForgetForm
{
	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;
}