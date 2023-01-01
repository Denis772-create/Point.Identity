namespace Point.Services.Identity.Application.Services;

public class KeyService : IKeyService
{
    protected readonly IKeyRepository _keyRepository;
    protected readonly IKeyServiceResources KeyServiceResources;

    public KeyService(IKeyRepository keyRepository, 
        IKeyServiceResources keyServiceResources)
    {
        _keyRepository = keyRepository;
        KeyServiceResources = keyServiceResources;
    }

    public async Task<KeysDto> GetKeysAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var keys = await _keyRepository.GetKeys(page, pageSize, cancellationToken);

        var keysDto = keys.ToModel();

        // TODO: create audit log

        return keysDto;
    }

    public async Task<KeyDto> GetKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        var key = await _keyRepository.GetKey(id, cancellationToken);

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
        return _keyRepository.ExistsKey(id, cancellationToken);
    }

    public async Task DeleteKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        var key = await GetKeyAsync(id, cancellationToken);

        // TODO: create audit log

        await _keyRepository.DeleteKeyAsync(id, cancellationToken);
    }
}