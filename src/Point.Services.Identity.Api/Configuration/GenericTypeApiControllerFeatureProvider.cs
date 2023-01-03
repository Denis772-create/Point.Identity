namespace Point.Services.Identity.Web.Configuration;

public class GenericTypeApiControllerFeatureProvider<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto> : IApplicationFeatureProvider<ControllerFeature>
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
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
        where TUserClaimDto : UserClaimDto<TKey>
        where TRoleClaimDto : RoleClaimDto<TKey>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var currentAssembly = typeof(GenericTypeApiControllerFeatureProvider<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>).Assembly;

        var controllerTypes = currentAssembly.GetExportedTypes()
                                             .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && t.IsGenericTypeDefinition)
                                             .Select(t => t.GetTypeInfo());

        var type = GetType();
        var genericType = type.GetGenericTypeDefinition().GetTypeInfo();
        var parameters = genericType.GenericTypeParameters
                                    .Select((p, i) => new { p.Name, Index = i })
                                    .ToDictionary(a => a.Name, a => type.GenericTypeArguments[a.Index]);

        foreach (var controllerType in controllerTypes)
        {
            var typeArguments = controllerType.GenericTypeParameters
                                              .Select(p => parameters[p.Name])
                                              .ToArray();

            feature.Controllers.Add(controllerType.MakeGenericType(typeArguments).GetTypeInfo());
        }
    }
}