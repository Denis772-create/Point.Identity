using Point.Services.Identity.Application.DTOs.Identity.Base;
using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity;

public class UserProviderDto<TKey> : BaseUserProviderDto<TKey>, IUserProviderDto
{
    public string UserName { get; set; }

    public string ProviderKey { get; set; }

    public string LoginProvider { get; set; }

    public string ProviderDisplayName { get; set; }
}