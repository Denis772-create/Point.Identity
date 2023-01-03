namespace Point.Services.Identity.Web.Controllers;

/// <summary>
/// This controller allows a user to revoke grants given to clients
/// </summary>
[SecurityHeaders]
[Authorize]
public class GrantsController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clients;
    private readonly IResourceStore _resources;

    public GrantsController(IIdentityServerInteractionService interaction,
        IClientStore clients,
        IResourceStore resources)
    {
        _interaction = interaction;
        _clients = clients;
        _resources = resources;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View("Index", await BuildViewModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revoke(string clientId)
    {
        await _interaction.RevokeUserConsentAsync(clientId);
        // TODO: rise event

        return RedirectToAction("Index");
    }

    private async Task<GrantsViewModel> BuildViewModelAsync()
    {
        var grants = await _interaction.GetAllUserGrantsAsync();

        var list = new List<GrantViewModel>();
        foreach (var grant in grants)
        {
            var client = await _clients.FindClientByIdAsync(grant.ClientId);
            if (client == null) continue;

            var resources = await _resources.FindResourcesByScopeAsync(grant.Scopes);

            var item = new GrantViewModel
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName ?? client.ClientId,
                ClientLogoUrl = client.LogoUri,
                ClientUrl = client.ClientUri,
                Description = grant.Description,
                Created = grant.CreationTime,
                Expires = grant.Expiration,
                IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
            };

            list.Add(item);
        }
        return new GrantsViewModel
        {
            Grants = list
        };
    }
}