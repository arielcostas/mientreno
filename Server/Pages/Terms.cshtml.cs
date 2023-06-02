using System.Globalization;
using Markdig;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Compartido;

namespace Mientreno.Server.Pages;

public class Terms : PageModel
{
	public string TermsHtml { get; set; } = string.Empty;
	
	public void OnGet()
	{
		var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
		var culture = rqf?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;

		var md = TermsProvider.GetTerms(Constantes.VersionTos, culture.TwoLetterISOLanguageName);
		
		TermsHtml = Markdown.ToHtml(md);
	}
}