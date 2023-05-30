using System.ComponentModel.DataAnnotations;
using Mientreno.Server.Service;

namespace Mientreno.Server.Data.Models;

public class Invitacion
{
	[Key] public string Id { get; set; } = Random.Shared.NextString(5);
	
	public Entrenador Entrenador { get; set; }
	public byte Usos { get; set; }
	public byte MaximoUsos { get; set; }
	public DateTime FechaExpiracion { get; set; } = DateTime.UtcNow.AddDays(3);
	
	public bool Usable => Usos < MaximoUsos && FechaExpiracion > DateTime.UtcNow;
}