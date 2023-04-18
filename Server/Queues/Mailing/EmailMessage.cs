namespace Mientreno.Server.Queues.Mailing;

#pragma warning disable CS8618

class EmailMessage
{
    public string NombreDestino { get; set; }
    public string DireccionDestino { get; set; }
    public string Idioma { get; set; }
    public string Plantila { get; set; }
    public string[] Parametros { get; set; }
}
