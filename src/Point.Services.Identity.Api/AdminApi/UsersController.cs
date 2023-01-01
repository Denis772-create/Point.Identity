namespace Point.Services.Identity.Web.AdminApi;

[Route("api/[controller]")]
[ApiController]
[TypeFilter(typeof(ControllerExceptionFilterAttribute))]
[Produces("application/json", "application/problem+json")]
[Authorize(Policy = ConfigurationConsts.AdministrationPolicy)]
public class UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto> : ControllerBase
    where TUserDto : UserDto<TKey>, new()
    where TRoleDto : RoleDto<TKey>, new()
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TUserToken : IdentityUserToken<TKey>
    where TUsersDto : UsersDto<TUserDto, TKey>
    where TRolesDto : RolesDto<TRoleDto, TKey>
    where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
    where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>, new()
    where TUserProviderDto : UserProviderDto<TKey>
    where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
    where TUserChangePasswordDto : UserChangePasswordDto<TKey>
    where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
    where TUserClaimDto : UserClaimDto<TKey>
    where TRoleClaimDto : RoleClaimDto<TKey>
{

}