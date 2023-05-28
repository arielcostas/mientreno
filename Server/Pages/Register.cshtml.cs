using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Compartido.Recursos;
using Mientreno.Server.Helpers;
using Mientreno.Server.Helpers.Queue;
using Mientreno.Server.Models;

namespace Mientreno.Server.Pages;

public class RegisterModel : PageModel
{
	private readonly ApplicationContext _context;
	private readonly UserManager<Usuario> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IQueueProvider _queueProvider;

	public RegisterModel(UserManager<Usuario> userManager, IQueueProvider queueProvider,
		RoleManager<IdentityRole> roleManager, ApplicationContext context)
	{
		_userManager = userManager;
		_queueProvider = queueProvider;
		_roleManager = roleManager;
		_context = context;
	}

	[FromQuery(Name = "inv")] public string? InvitacionCode { get; set; } = string.Empty;
	public Invitacion? Invitacion { get; set; }

	[BindProperty] public RegisterForm Form { get; set; } = new();

	public bool EmailSent { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		if (string.IsNullOrEmpty(InvitacionCode)) return Page();

		var invitacion = await _context.Invitaciones
			.Include(i => i.Entrenador)
			.FirstOrDefaultAsync(i => i.Id == InvitacionCode);

		if (invitacion is null || !invitacion.Usable)
		{
			Console.WriteLine(invitacion.Usable);
			return Page();
		}

		Invitacion = invitacion;
		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		if (!await _roleManager.RoleExistsAsync(Entrenador.RoleName))
		{
			await _roleManager.CreateAsync(new IdentityRole(Entrenador.RoleName));
		}

		if (!await _roleManager.RoleExistsAsync(Alumno.RoleName))
		{
			await _roleManager.CreateAsync(new IdentityRole(Alumno.RoleName));
		}

		Usuario nuevo;
		if (InvitacionCode.IsNullOrEmpty())
		{
			nuevo = new Entrenador
			{
				Nombre = Form.Nombre,
				Apellidos = Form.Apellidos,
				FechaAlta = DateTime.Now,

				UserName = Form.Email,
				Email = Form.Email
			};
		}
		else
		{
			var invitacion = await _context.Invitaciones
				.Include(i => i.Entrenador)
				.FirstOrDefaultAsync(i => i.Id == InvitacionCode);

			if (invitacion is null || !invitacion.Usable) return Forbid();
			
			nuevo = new Alumno
			{
				Nombre = Form.Nombre,
				Apellidos = Form.Apellidos,
				FechaAlta = DateTime.Now,
				Entrenador = invitacion.Entrenador,

				UserName = Form.Email,
				Email = Form.Email
			};
			
			invitacion.Usos += 1;
			await _context.SaveChangesAsync();
		}
		
		var result = await _userManager.CreateAsync(nuevo, Form.Contraseña);

		var rol = nuevo is Entrenador ? Entrenador.RoleName : Alumno.RoleName;
		await _userManager.AddToRoleAsync(nuevo, rol);

		foreach (var error in result.Errors)
		{
			Console.WriteLine(@$"{error.Code}: {error.Description}");
			ModelState.AddModelError(string.Empty, error.Description);
		}

		if (result.Errors.Any())
		{
			return Page();
		}

		var urlConfirmacion = Url.Page(
			"/Confirm",
			null,
			new { nuevo.Id, token = GenerateEmailConfirmationToken(nuevo) },
			Request.Scheme
		);

		var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
		var culture = rqf?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;

		_queueProvider.Enqueue(Constantes.ColaEmails, new Email()
		{
			Idioma = culture.TwoLetterISOLanguageName,
			NombreDestinatario = $"{Form.Nombre} {Form.Apellidos}",
			DireccionDestinatario = Form.Email,
			Plantila = nuevo is Entrenador
				? Constantes.EmailConfirmar
				: Constantes.EmailConfirmarAlumno,
			Parametros = new[] { Form.Nombre, urlConfirmacion! }
		});

		EmailSent = true;
		
		return Page();
	}

	private string GenerateEmailConfirmationToken(Usuario user)
	{
		return _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
	}
}

public class RegisterForm
{
	[Required] [MinLength(2)] public string Nombre { get; set; } = string.Empty;

	[Required] [MinLength(2)] public string Apellidos { get; set; } = string.Empty;

	[Required] [MinLength(2)] public string Email { get; set; } = string.Empty;

	[Required] public string Contraseña { get; set; } = string.Empty;

	[Required]
	[Compare(nameof(Contraseña),
		ErrorMessageResourceType = typeof(AppStrings),
		ErrorMessageResourceName = nameof(AppStrings.passwordsDoNotMatch)
	)]
	public string ConfirmarContraseña { get; set; } = string.Empty;

	[Required] public bool AceptoTerminos { get; set; } = false;
}