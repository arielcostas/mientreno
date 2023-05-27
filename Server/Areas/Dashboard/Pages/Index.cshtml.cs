using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mientreno.Server.Models;

namespace Mientreno.Server.Areas.Dashboard.Pages;

public class DashboardModel : PageModel
{
	private readonly UserManager<Usuario> _userManager;
	
	public DashboardModel(UserManager<Usuario> userManager)
	{
		_userManager = userManager;
	}

	public Subscription UserSubscription;
	public Entrenador Entrenador;
	public List<AlumnoEvent> Events;

	public async Task<IActionResult> OnGet()
	{
		UserSubscription = new Subscription("Basic", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(20), true);
		Entrenador = (await _userManager.GetUserAsync(User) as Entrenador)!;
		Events = new List<AlumnoEvent>
		{
			new("Ariel", EventType.TrainingComplete, DateTime.Now.AddMinutes(-10)),
			new("Juan", EventType.TrainingMissed, DateTime.Now.AddMinutes(-75)),
			new("Alberto", EventType.FeedbackReceived, DateTime.Now.AddHours(-4)),
			new("Alberto", EventType.TrainingComplete, DateTime.Now.AddHours(-4)),
		};

		return Page();
	}
}

public record Subscription(
	string PlanName,
	DateTime StartDate,
	DateTime EndDate,
	bool IsAutoRenewal
)
{
	public int PercentageCompleted => (int) Math.Round((DateTime.Now - StartDate).TotalDays / (EndDate - StartDate).TotalDays * 100);
}

public record AlumnoEvent(
	string AlumnoName,
	EventType EventType,
	DateTime EventDate
)
{
	public TimeSpan TimeSinceEvent => DateTime.Now - EventDate;
}

public enum EventType
{
	TrainingComplete,
	TrainingMissed,
	FeedbackReceived,
}