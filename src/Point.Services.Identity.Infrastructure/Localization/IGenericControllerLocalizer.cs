using Microsoft.Extensions.Localization;

namespace Point.Services.Identity.Infrastructure.Localization;

public interface IGenericLocalizer<out T>
{
    LocalizedString this[string name] { get; }

    LocalizedString this[string name, params object[] arguments] { get; }

    IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);
}