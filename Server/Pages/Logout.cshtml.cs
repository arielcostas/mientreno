using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Data.Models;
using Mientreno.Server.Service.Queue;

namespace Mientreno.Server.Pages;

public class LogoutModel : PageModel
{
	private readonly SignInManager<Usuario> _signInManager;

	public LogoutModel(SignInManager<Usuario> signInManager)
	{
		_signInManager = signInManager;
	}
	public async Task<IActionResult> OnGet()
	{
		await _signInManager.SignOutAsync();
		return RedirectToPage("/Login");
	}
}