using Markdig;
using Mientreno.Server.Models;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace Mientreno.Server.Helpers.Mailing;

/// <summary>
/// It generates an <see cref="Email">Email</see> with the email message to be sent according to the template and the language.
/// </summary>
public partial class EmailTemplate
{
    private static readonly string _templatePath =
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"EmailTemplates");

    private static readonly MarkdownPipeline pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

    /// <summary>
    /// Email enviado para confirmar la dirección de correo electrónico.
    /// </summary>
    /// <returns>An email that can be enqueued to  the MailWorkerService.</returns>
    public static Email ConfirmAddressEmail(ref Usuario usuario)
    {
        Console.WriteLine(_templatePath);

        var (subj, plain, html) = ApplyTemplate(
            "Confirm",
            "es",
            new object[] {
                usuario.Nombre,
                usuario.Credenciales.CodigoVerificacionEmail!,
                UrlEncoder.Default.Encode(usuario.Credenciales.Email)
            }
        );

        return new(
            $"{usuario.Nombre} {usuario.Apellidos}",
            usuario.Credenciales.Email, 
            subj,
            plain,
            html
        );
    }

    /// <summary>
    /// Email enviado para confirmar la dirección de correo electrónico.
    /// </summary>
    /// <returns>An email that can be enqueued to  the MailWorkerService.</returns>
    public static Email AfterConfirmWelcomeEmail(ref Usuario usuario)
    {
        var (subj, plain, html) = ApplyTemplate(
            "Welcome",
            "es",
            new object[] {
                usuario.Nombre,
                usuario.Credenciales.CodigoVerificacionEmail!,
                UrlEncoder.Default.Encode(usuario.Credenciales.Email)
            }
        );

        return new(
            $"{usuario.Nombre} {usuario.Apellidos}",
            usuario.Credenciales.Email,
            subj,
            plain,
            html
        );
    }

    public static (string subject, string plain, string html) ApplyTemplate(
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
