using Mientreno.Server.Data.Models;

namespace Mientreno.Server.Service;

public class SubscriptionRestrictions
{
	public static int MaxAlumnosPerEntrenador(PlanSuscripcion? plan)
	{
		return plan switch
		{
			PlanSuscripcion.Basic => 2,
			PlanSuscripcion.Estandar => 5,
			PlanSuscripcion.Prime => 20,
			null => 0,
			_ => throw new ArgumentOutOfRangeException(nameof(plan), plan, null)
		};
	}
	
	public static bool PrioritySupport(PlanSuscripcion plan)
	{
		return plan switch
		{
			PlanSuscripcion.Basic => false,
			PlanSuscripcion.Estandar => true,
			PlanSuscripcion.Prime => true,
			_ => throw new ArgumentOutOfRangeException(nameof(plan), plan, null)
		};
	}
}