namespace Point.Services.Identity.Web.DTOs.Users;

public class UserRoleApiDto<TKey>
{
    public TKey UserId { get; set; }

    public TKey RoleId { get; set; }
}