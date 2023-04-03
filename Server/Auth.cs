using Microsoft.AspNetCore.Authorization;
using Mientreno.Server.Helpers;

namespace Mientreno.Server;

internal class ValidSessionKeyAuthorizationHandler : AuthorizationHandler<ValidSessionKeyRequirement>
{
    private AppDbContext _context;

    public ValidSessionKeyAuthorizationHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidSessionKeyRequirement requirement)
    {
        // nonce user claim
        var nonce = context.User.Claims.First(x => x.Type == "nonce").Value;

        var found = _context.Sesiones.Where(s => !s.Invalidada).FirstOrDefault(s => s.SessionId == nonce);

        if (found == null)
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

internal class ValidSessionKeyRequirement : IAuthorizationRequirement
{
    public ValidSessionKeyRequirement()
    {
    }
}