namespace Point.Services.Identity.Application.Resources;

public class KeyServiceResources : IKeyServiceResources
{
    public ResourceMessage KeyDoesNotExist()
    {
        return new ResourceMessage
        {
            Code = nameof(KeyDoesNotExist),
            Description = KeyServiceResource.KeyDoesNotExist
        };
    }
}