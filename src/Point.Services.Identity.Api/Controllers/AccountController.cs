using Point.Services.Identity.Web.Configuration;
using Point.Services.Identity.Web.ViewModels.Account;

namespace Point.Services.Identity.Web.Controllers;

[Authorize]
[SecurityHeaders]
public class AccountController<TUser> : Controller
    where TUser : IdentityUser<Guid>, new()
{

    private readonly IClientStore _clientStore;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<TUser> _userManager;
    private readonly IdentityOptions _identityOptions;
    private readonly UserResolver<TUser> _userResolver;
    private readonly LoginConfiguration _loginConfiguration;
    private readonly ILogger<AccountController<TUser>> _logger;
    private readonly RegisterConfiguration _registerConfiguration;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly ApplicationSignInManager<TUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IGenericLocalizer<AccountController<TUser>> _localizer;


    public AccountController(IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IClientStore clientStore,
        ApplicationSignInManager<TUser> signInManager,
        IGenericLocalizer<AccountController<TUser>> localizer,
        UserManager<TUser> userManager,
        UserResolver<TUser> userResolver,
        LoginConfiguration loginConfiguration,
        RegisterConfiguration registerConfiguration,
        IEmailSender emailSender, IdentityOptions identityOptions, ILogger<AccountController<TUser>> logger)
    {
        _interaction = interaction;
        _schemeProvider = schemeProvider;
        _clientStore = clientStore;
        _signInManager = signInManager;
        _localizer = localizer;
        _userManager = userManager;
        _userResolver = userResolver;
        _loginConfiguration = loginConfiguration;
        _registerConfiguration = registerConfiguration;
        _emailSender = emailSender;
        _identityOptions = identityOptions;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var vm = await BuildLoginViewModelAsync(returnUrl);

        if (vm.EnableLocalLogin == false && vm.ExternalProviders.Count() == 1)
        {
            return ExternalLogin(vm.ExternalProviders.First().AuthenticationScheme, returnUrl);
        }

        return View(vm);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel model, string button)
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        if (button != "login")
        {
            if (context != null)
            {
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return context.IsNativeClient()
                    ? this.LoadingPage("Redirect", model.ReturnUrl)
                    : Redirect(model.ReturnUrl);
            }
        }

        if (ModelState.IsValid)
        {
            var user = await _userResolver.GetUserAsync(model.Username!);
            if (user != default(TUser))
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password,
                    model.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    // TODO: raise event here User Login Success

                    if (context != null)
                    {
                        return context.IsNativeClient()
                            ? this.LoadingPage("Redirect", model.ReturnUrl)
                            : Redirect(model.ReturnUrl);
                    }

                    if (Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);

                    if (string.IsNullOrEmpty(model.ReturnUrl)) return Redirect("~/");

                    throw new Exception("invalid return URL");
                }

                if (result.RequiresTwoFactor)
                {
                    // TODO: implement redirection to Login With 2fa
                }

                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
            }
            // TODO: raise event here User Login Failure
            ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);

        }
        var vm = await BuildLoginViewModelAsync(model);
        return View(vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var vm = await BuildLogoutViewModelAsync(logoutId);

        if (vm.ShowLogoutPrompt == false) return await Logout(vm);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutInputModel model)
    {
        var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

        if (User.Identity?.IsAuthenticated == true)
        {
            await _signInManager.SignOutAsync();

            // TODO: rise event User Logout
        }

        // check if we need to trigger sign-out at an upstream identity provider
        if (vm.TriggerExternalSignout)
        {
            var url = Url.Action("Logout", new { logoutId = vm.LogoutId });

            // this triggers a redirect to the external provider for sign-out
            return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
        }

        return View("LoggedOut", vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (!_registerConfiguration.Enabled) return View("RegisterFailure");

        ViewData["ReturnUrl"] = returnUrl;

        return _loginConfiguration.ResolutionPolicy switch
        {
            LoginResolutionPolicy.Username => View(),
            LoginResolutionPolicy.Email => View("RegisterWithoutUsername"),
            _ => View("RegisterFailure")
        };
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null, bool isCalledFromRegisterWithoutUsername = false)
    {
        if (!_registerConfiguration.Enabled) return View("RegisterFailure");

        returnUrl ??= Url.Content("~/");

        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid) return View(model);

        var user = new TUser
        {
            UserName = model.UserName,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new
            {
                userId = user.Id,
                code
            }, HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email,
                _localizer["ConfirmEmailTitle"],
                _localizer["ConfirmEmailBody",
                    HtmlEncoder.Default.Encode(callbackUrl!)]);

            if (_identityOptions.SignIn.RequireConfirmedAccount)
            {
                return View("RegisterConfirmation");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        AddErrors(result);

        if (isCalledFromRegisterWithoutUsername)
        {
            var registerWithoutUsernameModel = new RegisterWithoutUsernameViewModel
            {
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            return View("RegisterWithoutUsername", registerWithoutUsernameModel);
        }
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
    {
        if (userId == null || code == null)
        {
            return View("Error");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ConfirmEmailAsync(user, code);
        return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            TUser user = null!;
            switch (model.Policy)
            {
                case LoginResolutionPolicy.Email:
                    try
                    {
                        user = await _userManager.FindByEmailAsync(model.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error retrieving user by email ({0}) for forgot password functionality: {1}", model.Email, ex.Message);
                        user = null;
                    }
                    break;
                case LoginResolutionPolicy.Username:
                    try
                    {
                        user = await _userManager.FindByNameAsync(model.Username);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error retrieving user by userName ({0}) for forgot password functionality: {1}", model.Username, ex.Message);
                    }
                    break;
            }

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return View("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id, code
            }, HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, 
                _localizer["ResetPasswordTitle"], 
                _localizer["ResetPasswordBody", 
                    HtmlEncoder.Default.Encode(callbackUrl)]);

            return View("ForgotPasswordConfirmation");

        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? code = null)
    {
        return code == null ? View("Error") : View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
        var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
        }

        AddErrors(result);

        return View();
    }

    [HttpPost]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, _localizer["ErrorExternalProvider", remoteError]);

            return View(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (result.Succeeded) return RedirectToLocal(returnUrl);

        if (result.RequiresTwoFactor)
        {
            // TODO: implement Login With 2fa
        }

        if (result.IsLockedOut) return View("Lockout");

        // If the user does not have an account, then ask the user to create an account.
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["LoginProvider"] = info.LoginProvider;
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var userName = info.Principal?.Identity?.Name;

        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email, UserName = userName });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
        string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        // Get the information about the user from the external login provider
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return View("ExternalLoginFailure");
        }

        if (ModelState.IsValid)
        {
            var user = new TUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToLocal(returnUrl);
                }
            }

            AddErrors(result);
        }

        ViewData["LoginProvider"] = info.LoginProvider;
        ViewData["ReturnUrl"] = returnUrl;

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterWithoutUsername(RegisterWithoutUsernameViewModel model, string returnUrl = null)
    {
        var registerModel = new RegisterViewModel
        {
            UserName = model.Email,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword
        };

        return await Register(registerModel, returnUrl, true);
    }


    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;


            var vm = new LoginViewModel
            {
                EnableLocalLogin = local,
                ReturnUrl = returnUrl,
                Username = context.LoginHint,
            };

            if (!local)
            {
                vm.ExternalProviders = new[]
                {
                    new ExternalProvider { AuthenticationScheme = context.IdP }
                };

                return vm;
            }
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var allowLocal = true;
        if (context?.Client.ClientId != null)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;

                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                {
                    providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }
        }

        return new LoginViewModel
        {
            AllowRememberLogin = AccountOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
            ReturnUrl = returnUrl,
            Username = context?.LoginHint,
            ExternalProviders = providers.ToArray()
        };
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
    {
        var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
        vm.Username = model.Username;
        vm.RememberLogin = model.RememberLogin;
        return vm;
    }

    private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
    {
        var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

        if (User.Identity?.IsAuthenticated != true)
        {
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        var context = await _interaction.GetLogoutContextAsync(logoutId);
        if (context?.ShowSignoutPrompt != false) return vm;

        vm.ShowLogoutPrompt = false;
        return vm;

    }

    private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
    {
        var logout = await _interaction.GetLogoutContextAsync(logoutId);

        var vm = new LoggedOutViewModel
        {
            AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
            PostLogoutRedirectUri = logout.PostLogoutRedirectUri,
            ClientName = string.IsNullOrEmpty(logout.ClientName) ? logout.ClientId : logout.ClientName,
            SignOutIframeUrl = logout.SignOutIFrameUrl,
            LogoutId = logoutId
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                if (providerSupportsSignout)
                {
                    vm.LogoutId ??= await _interaction.CreateLogoutContextAsync();

                    vm.ExternalAuthenticationScheme = idp;
                }
            }
        }

        return vm;
    }
}