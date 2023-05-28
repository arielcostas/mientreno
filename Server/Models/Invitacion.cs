﻿using System.ComponentModel.DataAnnotations;
using System.Text;
using Mientreno.Server.Helpers;

namespace Mientreno.Server.Models;

public class Invitacion
{
	[Key] public string Id { get; set; } = Random.Shared.NextString(5);
	
	public Entrenador Entrenador { get; set; }
	public byte Usos { get; set; }
	public byte MaximoUsos { get; set; }
	public DateTime FechaExpiracion { get; set; } = DateTime.UtcNow.AddDays(3);
	
	public bool Usable => Usos < MaximoUsos && FechaExpiracion > DateTime.UtcNow;
}