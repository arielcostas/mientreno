﻿using System.Text;

namespace Server.Helpers;

public static class SigningKeyHolder
{
    private static string? _token;
    private static void Initialize()
    {
        // TODO: Make this safer in production
        var b = Encoding.Default.GetBytes(Environment.MachineName);
        _token = Convert.ToHexString(b);
        Console.WriteLine($"Token: {_token}");
    }

    public static byte[] GetToken()
    {
        if (_token == null)
        {
            Initialize();
        }

        return Encoding.Default.GetBytes(_token!);
    }

    public static string GetTokenString()
    {
        if ( _token == null )
        {
            Initialize();
        }

        return _token!;
    }
}