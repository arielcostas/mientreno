using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Mientreno.Server.Helpers.Services;
using Mientreno.Server.Models;
using System.Reflection;

namespace Mientreno.Server.Helpers.Mailing;

/// <summary>
/// It generates an <see cref="Helpers.Services.Email">Email</see> with the email message to be sent according to the template and the language.
/// </summary>
public class MailTemplate
{
    private readonly string _templatePath = 
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"EmailTemplates");


    /// <summary>
    /// Email enviado para confirmar la dirección de correo electrónico.
    /// </summary>
    /// <returns>An email that can be enqueued to  the MailWorkerService.</returns>
    public Email ConfirmAddressEmail(ref Usuario usuario)
    {
        return new(
            usuario.Credenciales.Email,
            "Confirma tu ",
            ""
        );
    }

    public Email ApplyTemplate(
        string template,
        string twoLetterLanguage,
        object[] parameters
    )
    {
        var localizedFileExists = File.Exists(_templatePath + $"\\{template}.{twoLetterLanguage}.md");

    }
}
