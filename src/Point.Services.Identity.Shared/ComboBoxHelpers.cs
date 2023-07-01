using Newtonsoft.Json;

namespace Point.Services.Identity.Shared;

public static class ComboBoxHelpers
{
    public static void PopulateValuesToList(string jsonValues, List<string> list)
    {
        if (string.IsNullOrEmpty(jsonValues)) return;

        var listValues = JsonConvert.DeserializeObject<List<string>>(jsonValues);
        if (listValues == null) return;

        list.AddRange(listValues);
    }
}