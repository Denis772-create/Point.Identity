namespace Point.Services.Identity.Application.Resources;

public interface IIdentityResourceServiceResources
{
    ResourceMessage IdentityResourceDoesNotExist();

    ResourceMessage IdentityResourceExistsKey();

    ResourceMessage IdentityResourceExistsValue();

    ResourceMessage IdentityResourcePropertyDoesNotExist();

    ResourceMessage IdentityResourcePropertyExistsValue();

    ResourceMessage IdentityResourcePropertyExistsKey();
}