namespace Mientreno.Compartido;

public class TermsProvider
{
	private static readonly string TemplatePath =
		Path.Combine(AppContext.BaseDirectory, @"TermsService");
	
	public static string GetTerms(uint version, string? language)
	{
		language ??= Idiomas.Castellano;
		var localizedFileExists = File.Exists(
			Path.Combine(TemplatePath, $"v{version}.{language}.md")
		);

		var tosFile = localizedFileExists
			? Path.Combine(TemplatePath, $"v{version}.{language}.md")
			: Path.Combine(TemplatePath, $"v{version}.es.md");

		if (!File.Exists(tosFile))
		{
			throw new FileNotFoundException($"Terms of service file {tosFile} not found.");
		}

		return File.ReadAllText(tosFile);

	}
}