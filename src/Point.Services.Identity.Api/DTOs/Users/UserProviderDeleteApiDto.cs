namespace Point.Services.Identity.Web.DTOs.Users;

public class UserProviderDeleteApiDto<TKey>
{
    public TKey UserId { get; set; }

    public string ProviderKey { get; set; }

    public string LoginProvider { get; set; }
}