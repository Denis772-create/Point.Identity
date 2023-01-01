namespace Point.Services.Identity.Application.Services;

public class KeyService : IKeyService
{
    protected readonly IKeyRepository KeyRepository;
    protected readonly IKeyServiceResources KeyServiceResources;

    public KeyService(IKeyRepository keyRepository, 
        IKeyServiceResources keyServiceResources)
    {
        KeyRepository = keyRepository;
        KeyServiceResources = keyServiceResources;
    }

    public async Task<KeysDto> GetKeysAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var keys = await KeyRepository.GetKeys(page, pageSize, cancellationToken);

        var keysDto = keys.ToModel();

        // TODO: create audit log

        return keysDto;
    }

    public async Task<KeyDto> GetKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        var key = await KeyRepository.GetKey(id, cancellationToken);

        if (key == default)
        {
            throw new UserFriendlyErrorPageException(string.Format(KeyServiceResources.KeyDoesNotExist().Description, id));
        }

        var keyDto = key.ToModel();

        // TODO: create audit log

        return keyDto;
    }

    public Task<bool> ExistsKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        return KeyRepository.ExistsKey(id, cancellationToken);
    }
}