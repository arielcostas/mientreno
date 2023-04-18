using Markdig;
using System.Text.RegularExpressions;

namespace Mientreno.Server.Helpers.Mailing;

public partial class EmailTemplateEngine
{
    private static readonly string _templatePath =
        Path.Combine(AppContext.BaseDirectory, @"EmailTemplates");

    private static readonly MarkdownPipeline pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    
    public static (string subject, string plain, string html) Apply(
        string template,
        string twoLetterLanguage,
        object[] parameters
    )
    {
        var localizedFileExists = File.Exists(_templatePath + $"\\{template}\\{twoLetterLanguage}.md");

        var templateFile = localizedFileExists
            ? $"{template}\\{twoLetterLanguage}.md"
            : $"{template}\\default.md";

        if (!File.Exists(_templatePath + $"\\{templateFile}"))
        {
            throw new FileNotFoundException($"Template file {templateFile} not found.");
        }

        var templateContent = File.ReadAllText(_templatePath + $"\\{templateFile}");

        string appliedTemplate = string.Format(templateContent, parameters);
        var parts = appliedTemplate.Split(Environment.NewLine, 2);

        string subject = MyRegex().Replace(parts[0], "");
        appliedTemplate = parts[1];

        string plain = Markdown.ToPlainText(appliedTemplate, pipeline);
        string html = Markdown.ToHtml(appliedTemplate, pipeline);

        return (subject, plain, html);
    }

    [GeneratedRegex("^#\\s?")]
    private static partial Regex MyRegex();
}
