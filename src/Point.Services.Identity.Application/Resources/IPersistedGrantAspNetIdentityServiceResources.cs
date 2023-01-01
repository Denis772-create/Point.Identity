namespace Point.Services.Identity.Application.Resources;

public interface IPersistedGrantAspNetIdentityServiceResources
{
    ResourceMessage PersistedGrantDoesNotExist();

    ResourceMessage PersistedGrantWithSubjectIdDoesNotExist();
}