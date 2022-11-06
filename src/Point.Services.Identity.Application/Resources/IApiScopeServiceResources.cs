namespace Point.Services.Identity.Application.Resources;

public interface IApiScopeServiceResources
{
    ResourceMessage ApiScopeDoesNotExist();
    ResourceMessage ApiScopeExistsValue();
    ResourceMessage ApiScopeExistsKey();
    ResourceMessage ApiScopePropertyExistsValue();
    ResourceMessage ApiScopePropertyDoesNotExist();
    ResourceMessage ApiScopePropertyExistsKey();
}