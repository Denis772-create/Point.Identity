namespace Point.Services.Identity.Web.DTOs.Users;

public class UserProvidersApiDto<TKey>
{
    public UserProvidersApiDto()
    {
        Providers = new List<UserProviderApiDto<TKey>>();
    }

    public List<UserProviderApiDto<TKey>> Providers { get; set; }
}