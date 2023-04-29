namespace Mientreno.Compartido.Peticiones;

public class MiEjercicio
{
	public int Id { get; set; }
	public string Nombre { get; set; }
	public string Descripcion { get; set; }
	public string? VideoUrl { get; set; }
	public int Dificultad { get; set; }
	public MiEjercicio__Categoria? Categoria { get; set; }

	public class MiEjercicio__Categoria
	{
		public int Id { get; set; }
		public string Nombre { get; set; }
	}
}

public class NuevoEjercicioOutput
{
	public int Id { get; set; }
}