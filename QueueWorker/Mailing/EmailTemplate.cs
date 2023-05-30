using System.Text.RegularExpressions;
using Markdig;

namespace Mientreno.QueueWorker.Mailing;

/// <summary>
/// It generates an <see cref="Email">Email</see> with the email message to be sent according to the template and the language.
/// </summary>
public partial class EmailTemplate
{
	private static readonly string _templatePath =
		Path.Combine(AppContext.BaseDirectory, @"EmailTemplates");

	private static readonly MarkdownPipeline pipeline =
		new MarkdownPipelineBuilder()
			.UseAdvancedExtensions()
			.Build();

	public static (string subject, string plain, string html) ApplyTemplate(
		string template,
		string twoLetterLanguage,
		object[] parameters
	)
	{
		var thisTemplatePath = Path.Combine(_templatePath, template);
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

		string appliedTemplate = string.Format(templateContent, parameters);
		var parts = appliedTemplate.Split("\n", 2);

		string subject = MyRegex().Replace(parts[0], "");
		appliedTemplate = parts[1];

		string plain = Markdown.ToPlainText(appliedTemplate, pipeline);
		string html = Markdown.ToHtml(appliedTemplate, pipeline);

		return (subject, plain, html);
	}

	[GeneratedRegex("^#\\s?")]
	private static partial Regex MyRegex();
}