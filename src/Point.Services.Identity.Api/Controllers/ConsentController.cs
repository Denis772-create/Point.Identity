namespace Point.Services.Identity.Web.Controllers;

/// <summary>
/// This controller processes the consent UI
/// </summary>
[SecurityHeaders]
[Authorize]
public class ConsentController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<ConsentController> _logger;

    public ConsentController(
        IIdentityServerInteractionService interaction,
        ILogger<ConsentController> logger)
    {
        _interaction = interaction;
        _logger = logger;
    }

    /// <summary>
    /// Shows the consent screen
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Index(string returnUrl)
    {
        var vm = await BuildViewModelAsync(returnUrl);
        return vm != null ? View("Index", vm) : View("Error");
    }


    /// <summary>
    /// Handles the consent screen postback
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ConsentInputModel model)
    {
        var result = await ProcessConsent(model);

        if (result.IsRedirect)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            return context?.IsNativeClient() == true
                ? this.LoadingPage("Redirect", result.RedirectUri)
                : Redirect(result.RedirectUri);
        }

        if (result.HasValidationError)
        {
            ModelState.AddModelError(string.Empty, result.ValidationError);
        }

        return result.ShowView
            ? View("Index", result.ViewModel)
            : View("Error");
    }

    private async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
    {
        var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (request != null)
        {
            return CreateConsentViewModel(model, returnUrl, request);
        }

        _logger.LogError("No consent request matching request: {0}", returnUrl);

        return null!;
    }

    private ConsentViewModel CreateConsentViewModel(
        ConsentInputModel model, string returnUrl,
        AuthorizationRequest request)
    {
        var vm = new ConsentViewModel
        {
            RememberConsent = model?.RememberConsent ?? true,
            ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
            Description = model?.Description,

            ReturnUrl = returnUrl,

            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent
        };

        vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources
            .Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null))
            .ToArray();

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                apiScopes.Add(scopeVm);
            }
            if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
            }
        }
        vm.ApiScopes = apiScopes;

        return vm;
    }

    private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new ScopeViewModel
        {
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };
    }

    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        var displayName = apiScope.DisplayName ?? apiScope.Name;
        if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
        {
            displayName += ":" + parsedScopeValue.ParsedParameter;
        }

        return new ScopeViewModel
        {
            Value = parsedScopeValue.RawValue,
            DisplayName = displayName,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    private static ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Value = IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }

    private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
    {
        var result = new ProcessConsentResult();

        // validate return url is still valid
        var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        if (request == null) return result;

        ConsentResponse grantedConsent = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (model?.Button == "no")
        {
            grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

            // TODO: rise event
        }
        // user clicked 'yes' - validate the data
        else if (model?.Button == "yes")
        {
            // if the user consented to some scope, build the response model
            if (model.ScopesConsented != null && model.ScopesConsented.Any())
            {
                var scopes = model.ScopesConsented;
                if (ConsentOptions.EnableOfflineAccess == false)
                {
                    scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
                }

                grantedConsent = new ConsentResponse
                {
                    RememberConsent = model.RememberConsent,
                    ScopesValuesConsented = scopes.ToArray(),
                    Description = model.Description
                };

                // TODO: rise event
            }
            else
            {
                result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
            }
        }
        else
        {
            result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
        }

        if (grantedConsent != null)
        {
            // communicate outcome of consent back to identityserver
            await _interaction.GrantConsentAsync(request, grantedConsent);

            // indicate that's it ok to redirect back to authorization endpoint
            result.RedirectUri = model.ReturnUrl;
            result.Client = request.Client;
        }
        else
        {
            // we need to redisplay the consent UI
            result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
        }

        return result;
    }
}