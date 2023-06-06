using System.Text.RegularExpressions;
using Markdig;

namespace Mientreno.QueueWorker.Mailing;

/// <summary>
/// It generates an <see cref="Email">Email</see> with the email message to be sent according to the template and the language.
/// </summary>
public partial class EmailTemplate
{
	private static readonly string TemplatePath =
		Path.Combine(AppContext.BaseDirectory, @"EmailTemplates");

	private static readonly MarkdownPipeline Pipeline =
		new MarkdownPipelineBuilder()
			.UseAdvancedExtensions()
			.Build();

	public static (string subject, string plain, string html) ApplyTemplate(
		string template,
		string twoLetterLanguage,
		object[] parameters
	)
	{
		var thisTemplatePath = Path.Combine(TemplatePath, template);
		var localizedFileExists = File.Exists(
			Path.Combine(thisTemplatePath, $"{twoLetterLanguage}.md")
		);

		var templateFile = localizedFileExists
			? Path.Combine(thisTemplatePath, $"{twoLetterLanguage}.md")
			: Path.Combine(thisTemplatePath, "default.md");

		if (!File.Exists(templateFile))
		{
			throw new FileNotFoundException($"Template file {templateFile} not found.");
		}

		var templateContent = File.ReadAllText(templateFile);

		var appliedTemplate = string.Format(templateContent, parameters);
		var parts = appliedTemplate.Split("\n", 2);

		var subject = MyRegex().Replace(parts[0], "");
		appliedTemplate = parts[1];

		var plain = Markdown.ToPlainText(appliedTemplate, Pipeline);
		var html = Markdown.ToHtml(appliedTemplate, Pipeline);

		// Aplicar estilos al HTML
		html = HtmlBase.Replace("{{body}}", html);

		return (subject, plain, html);
	}

	[GeneratedRegex("^#\\s?")]
	private static partial Regex MyRegex();

	private const string HtmlBase = """
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
	<meta name="viewport" content="width=device-width,initial-scale=1">
	<meta name="x-apple-disable-message-reformatting">

	<style>
		body {
			font-size: 16px;
			padding: 0 calc((100% - 80ch) / 2);
			text-align: justify;
		}
	</style>
</head>
<body>
	<main>{{body}}</main>
	<footer>
		<p>Copyright &copy; 2023 MiEntreno. Todos los derechos reservados.</p>
		<p>
			Este mensaje se ha enviado porque tiene una cuenta en <a href="https://mientreno.app">https://mientreno.app</a>.
			Por favor, contacte con <a href="mailto:hola@mientreno.app">hola@mientrneo.app</a>.
			Por favor, no responda a este mensaje.
		</p>
	</footer>
</body>
</html>
""";
}