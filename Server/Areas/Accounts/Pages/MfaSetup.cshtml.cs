using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mientreno.Server.Areas.Accounts.Services;
using Mientreno.Server.Areas.Dashboard.Services;
using Mientreno.Server.Data;
using Mientreno.Server.Data.Models;
using Net.Codecrete.QrCodeGenerator;

namespace Mientreno.Server.Areas.Accounts.Pages;

public class MfaSetupModel : UsuarioPageModel
{
	private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

	public MfaSetupModel(UserManager<Usuario> userManager, ApplicationDatabaseContext databaseContext) : base(
		userManager, databaseContext)
	{
	}

	[BindProperty] public MfaSetupForm Form { get; set; } = new();
	public string AuthUri { get; set; } = string.Empty;
	public string SharedKey { get; set; } = string.Empty;
	public string QrCodeSvg { get; set; } = string.Empty;

	public async Task<IActionResult> OnGetAsync()
	{
		LoadUsuario();
		await LoadSharedKeyAndQrCodeUriAsync();
		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		LoadUsuario();
		var verificationCode = Form.Code
			.Replace(" ", string.Empty)
			.Replace("-", string.Empty);
		
		if (!await UserManager.VerifyTwoFactorTokenAsync(Usuario, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode))
		{
			ModelState.AddModelError("Code", "Código incorrecto");
			await LoadSharedKeyAndQrCodeUriAsync();
			return Page();
		}
		
		await UserManager.SetTwoFactorEnabledAsync(Usuario, true);

		return RedirectToPage("/Index");
	}
		
	private async Task LoadSharedKeyAndQrCodeUriAsync()
	{
		var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(Usuario);

		if (string.IsNullOrEmpty(unformattedKey))
		{
			await UserManager.ResetAuthenticatorKeyAsync(Usuario);
			unformattedKey = await UserManager.GetAuthenticatorKeyAsync(Usuario);
		}

		SharedKey = FormatSharedKey(unformattedKey!);
		
		var email = await UserManager.GetEmailAsync(Usuario)!;
		AuthUri = GetSharedKeyUri(email!, unformattedKey!);

		var qr = QrCode.EncodeText(AuthUri, QrCode.Ecc.Medium);
		QrCodeSvg = qr.ToSvgString(1);
	}
	
	private static string GetSharedKeyUri(string email, string unformattedKey)
	{
		return string.Format(
			CultureInfo.InvariantCulture,
			AuthenticatorUriFormat,
			Uri.EscapeDataString("MiEntreno"),
			Uri.EscapeDataString(email),
			unformattedKey
		);
	}

	private static string FormatSharedKey(string sharedKey)
	{
		var result = new StringBuilder();
		int currentPosition = 0;
		while (currentPosition + 4 < sharedKey.Length)
		{
			result.Append(sharedKey.AsSpan(currentPosition, 4)).Append(' ');
			currentPosition += 4;
		}

		if (currentPosition < sharedKey.Length)
		{
			result.Append(sharedKey.AsSpan(currentPosition));
		}

		return result.ToString().ToUpperInvariant();
	}
}

public class MfaSetupForm
{
	public string Code { get; set; } = string.Empty;
}