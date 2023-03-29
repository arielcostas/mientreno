using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Server.Helpers;

public class TokenGenerator
{    
    /// <summary>
    /// El token de desafio es un hash HMAC-SHA256 de los datos del usuario y la fecha de expiración.
    /// Los datos se concatenan con dos exclamaciones (!!), y se calcula un HMAC-SHA256 con la clave privada.
    /// Se devuelve el conjunto de datos y el hash; concatenado y como base64.
    /// </summary>
    /// <returns></returns>
    public string GenerarTokenMac(DateTime expiraEn, IDictionary<string, string> datos)
    {
        var payload = string.Empty;
        foreach (var (key, value) in datos)
        {
            if (payload != string.Empty)
                payload += "!!";
            payload += $"{key}={value}";
        }
        
        payload += $"!!exp={expiraEn.Ticks}";

        // Generate a hash from the parts with HMAC-SHA256
        using var hmac = new HMACSHA256(SigningKeyHolder.GetToken());

        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        
        // Concatenate the parts and the hash, and return as base64
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}..{Convert.ToBase64String(hash)}"));
    }

    /// <summary>
    /// Descodifica y verifica la integridad del token de desafio generado por GenerarTokenDesafio.
    /// </summary>
    /// <returns>El ID de usuario</returns>
    public IDictionary<string, string> VerificarTokenMac(string token)
    {
        // Decode the token
        var parts = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split("..");
        
        // Verify the hash
        using var hmac = new HMACSHA256(SigningKeyHolder.GetToken());
        
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(parts[0]));
        
        if (Convert.ToBase64String(hash) != parts[1])
            throw new SecurityTokenException("Token inválido");
        
        // Parse the payload
        var payload = parts[0].Split("!!");
        var datos = new Dictionary<string, string>();
        foreach (var part in payload)
        {
            var kv = part.Split("=");
            datos.Add(kv[0], kv[1]);
        }
        
        return datos;
    }

    public string GenerarTokenAcceso(DateTime expiraEn, string id, string login, string rol, string nonce)
    {
        Claim[] claims =
        {
            new(ClaimTypes.NameIdentifier, id),
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, rol),
            new("nonce", nonce)
        };

        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = expiraEn,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(SigningKeyHolder.GetToken()),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var securityToken = new JwtSecurityTokenHandler().CreateToken(securityTokenDescriptor);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
    
    public string? VerificarTokenAcceso(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = SigningKeyHolder.GetToken();

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        }, out var validatedToken);

        var jwtToken = (JwtSecurityToken) validatedToken;

        return jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}