using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Server.Helpers;

public class TokenGenerator
{
    private readonly byte[] _tokenSigningKey = "abc"u8.ToArray();

    public string GenerarTokenAcceso(DateTime expiraEn, string id, string login, string rol)
    {
        return GenerarJsonWebToken(expiraEn, id, login, rol);
    }
    
    public string? VerificarTokenAcceso(string token)
    {
        return VerificarJsonWebToken(token);
    }

    public string GenerarTokenRefresco(DateTime expiraEn, string id, string login, string rol)
    {
        throw new NotImplementedException();
    }
    
    public string? VerificarTokenRefresco(string token)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// El token de desafio es un hash HMAC-SHA256 de los datos del usuario y la fecha de expiración.
    /// Los datos se concatenan con dos exclamaciones (!!), y se calcula un HMAC-SHA256 con la clave privada.
    /// Se devuelve el conjunto de datos y el hash; concatenado y como base64.
    /// </summary>
    /// <returns></returns>
    public string GenerarTokenDesafio(DateTime expiraEn, IDictionary<string, string> datos)
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
        using var hmac = new HMACSHA256(_tokenSigningKey);

        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        
        // Concatenate the parts and the hash, and return as base64
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}..{Convert.ToBase64String(hash)}"));
    }

    /// <summary>
    /// Descodifica y verifica la integridad del token de desafio generado por GenerarTokenDesafio.
    /// </summary>
    /// <returns>El ID de usuario</returns>
    public IDictionary<string, string> VerificarTokenDesafio(string token)
    {
        // Decode the token
        var parts = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split("..");
        
        // Verify the hash
        using var hmac = new HMACSHA256(_tokenSigningKey);
        
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(parts[0]));
        
        if (Convert.ToBase64String(hash) != parts[1])
            throw new SecurityTokenException("Token de desafío inválido");
        
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

    private string GenerarJsonWebToken(DateTime expiraEn, string id, string login, string rol)
    {
        Claim[] claims =
        {
            new(ClaimTypes.NameIdentifier, id),
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, rol)
        };

        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = expiraEn,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_tokenSigningKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var securityToken = new JwtSecurityTokenHandler().CreateToken(securityTokenDescriptor);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
    
    private string? VerificarJsonWebToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = _tokenSigningKey;

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