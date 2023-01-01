using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Point.Services.Identity.Application.Services;
using Point.Services.Identity.Infrastructure.Interfaces;
using Point.Services.Identity.Infrastructure.Repositories;
using Point.Services.Identity.Application.Mappers.Configuration;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Point.Services.Identity.Application.Extensions;

public static class AdminServicesExtensions
{
    public static IMapperConfigurationBuilder AddAdminAspNetIdentityMapping(this IServiceCollection services)
    {
        var builder = new MapperConfigurationBuilder();

        services.AddSingleton<IConfigurationProvider>(sp => new MapperConfiguration(cfg =>
        {
            foreach (var profileType in builder.ProfileTypes)
                cfg.AddProfile(profileType);
        }));

        services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

        return builder;
    }

    public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
        this IServiceCollection services, HashSet<Type> profileTypes)
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUserDto : UserDto<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRoleDto : RoleDto<TKey>
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
        //Repositories
        services.AddTransient<IIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, 
            IdentityRepository<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
        services.AddTransient<IPersistedGrantAspNetIdentityRepository, 
            PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();

        //Services
        services.AddTransient<IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>,
            IdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>>();
        services.AddTransient<IPersistedGrantAspNetIdentityService, PersistedGrantAspNetIdentityService>();

        //Resources
        services.AddScoped<IIdentityServiceResources, IdentityServiceResources>();
        services.AddScoped<IPersistedGrantAspNetIdentityServiceResources, PersistedGrantAspNetIdentityServiceResources>();

        //Register mapping
        services.AddAdminAspNetIdentityMapping()
            .UseIdentityMappingProfile<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim,
                TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto,
                TRoleClaimDto>()
            .AddProfilesType(profileTypes);

        return services;
    }

    public static IServiceCollection AddAdminServices<TConfigurationDbContext, TPersistedGrantDbContext>(this IServiceCollection services)
    where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
    where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
    {
        //Repositories
        services.AddTransient<IClientRepository, ClientRepository<TConfigurationDbContext>>();
        services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository<TConfigurationDbContext>>();
        services.AddTransient<IApiResourceRepository, ApiResourceRepository<TConfigurationDbContext>>();
        services.AddTransient<IApiScopeRepository, ApiScopeRepository<TConfigurationDbContext>>();
        services.AddTransient<IKeyRepository, KeyRepository<TPersistedGrantDbContext>>();
        // TODO: IPersistedGrantRepository

        //Services
        services.AddTransient<IClientService, ClientService>();
        services.AddTransient<IApiResourceService, ApiResourceService>();
        services.AddTransient<IApiScopeService, ApiScopeService>();
        services.AddTransient<IIdentityResourceService, IdentityResourceService>();
        services.AddTransient<IKeyService, KeyService>();
        // TODO: IPersistedGrantService

        //Resources
        services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
        services.AddScoped<IApiScopeServiceResources, ApiScopeServiceResources>();
        services.AddScoped<IClientServiceResources, ClientServiceResources>();
        services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
        services.AddScoped<IKeyServiceResources, KeyServiceResources>();
        // TODO: IPersistedGrantServiceResources

        return services;
    }

}