using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mientreno.Compartido.Errores;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Credenciales inválidas")
    {
    }
}

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("No se encontró el usuario con ese ID, nombre o email.")
    {
    }
}