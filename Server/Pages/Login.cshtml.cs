﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido.Recursos;
using Mientreno.Server.Models;

namespace Mientreno.Server.Pages;

public class LoginModel : PageModel
{
	private readonly SignInManager<Usuario> _signInManager;
	private readonly UserManager<Usuario> _userManager;

	public LoginModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
	{
		_signInManager = signInManager;
		_userManager = userManager;
	}

	[BindProperty] public LoginForm Form { get; set; } = new();
	
	public string MensajeError { get; set; } = string.Empty;

	public async Task<IActionResult> OnPost()
	{
		var user = await _userManager.FindByEmailAsync(Form.Email);
		
		if (user is null)
		{
			MensajeError = AppStrings.errorInvalidCredentials;
			return Page();
		}
		
		var res = await _signInManager.PasswordSignInAsync(
			user,
			Form.Contraseña,
			Form.Recordar,
			false
		);
		
		if (res.Succeeded) return RedirectToPage("/Index");
		
		MensajeError = AppStrings.errorInvalidCredentials;
		return Page();
	}
}

public class LoginForm
{
	public string Email { get; set; } = string.Empty;
	public string Contraseña { get; set; } = string.Empty;
	public bool Recordar { get; set; } = false;
}