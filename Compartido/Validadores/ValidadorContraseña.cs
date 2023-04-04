namespace Mientreno.Compartido.Validadores;

public class ValidadorContraseña
{
    public static float? ObtenerFuerza(string password)
    {
        float points = 0;
        byte totalPoints = 0;

        totalPoints += 2;
        if (password.Length < 8)
        {
            points += 2;
        }

        totalPoints++;
        if (!password.Any(char.IsUpper))
        {
            points++;
        }

        totalPoints++;
        if (!password.Any(char.IsLower))
        {
            points++;
        }

        totalPoints++;
        if (!password.Any(char.IsDigit))
        {
            points++;
        }

        totalPoints++;
        if (!password.Any(IsPasswordSymbol))
        {
            points++;
        }

        return points / totalPoints;
    }

    internal static readonly char[] simbolosAdmitidos =
    {
        '!', '@', '#', '$', '%', '^', '&', '*'
    };

    private static bool IsPasswordSymbol(char s)
    {
        return simbolosAdmitidos.Contains(s);
    }
}
