namespace Server.Models;

public class Habitos
{
    public string Profesion { get; set; }
    public Fumador Fumador { get; set; }
    public Bebedor Bebedor { get; set; }
    public string OtrasDrogas { get; set; }
    public Deporte Deporte { get; set; }
    public string Sueño { get; set; }
}

public enum Fumador
{
    No,
    Social,
    Ocasional,
    Dejando,
    Si
}

public enum Bebedor
{
    No,
    Social,
    Ocasional,
    Dejando,
    Si
}

public enum Deporte
{
    No,
    Ocasional,
    Semanal,
    Diario
}