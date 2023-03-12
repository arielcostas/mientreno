using System.Security.Claims;

namespace Server;

public class TokenGenerator
{
    public string GenerarToken(TipoToken tipo, DateTime expiraEn, string id, string login, string rol)
    {
        throw new NotImplementedException();
    }
    
}

public enum TipoToken
{
    Acceso,
    Refresco,
    Desafio 
}