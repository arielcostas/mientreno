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
    NO,
    SOCIAL,
    OCASIONAL,
    DEJANDO,
    SI
}

public enum Bebedor
{
    NO,
    SOCIAL,
    OCASIONAL,
    DEJANDO,
    SI
}

public enum Deporte
{
    NO,
    OCASIONAL,
    SEMANAL,
    DIARIO
}