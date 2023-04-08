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