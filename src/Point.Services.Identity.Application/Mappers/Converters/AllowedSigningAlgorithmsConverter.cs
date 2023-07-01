namespace Point.Services.Identity.Application.Mappers.Converters;

public class AllowedSigningAlgorithmsConverter :
    IValueConverter<List<string>, string>,
    IValueConverter<string, List<string>>
{
    public static AllowedSigningAlgorithmsConverter Converter = new();

    public string Convert(List<string> sourceMember, ResolutionContext context)
    {
        if (sourceMember == null || !sourceMember.Any())
        {
            return null;
        }
        return sourceMember.Aggregate((x, y) => $"{x},{y}");
    }

    public List<string> Convert(string sourceMember, ResolutionContext context)
    {
        var list = new List<string>();
        if (!string.IsNullOrWhiteSpace(sourceMember))
        {
            sourceMember = sourceMember.Trim();
            list.AddRange(sourceMember.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct());
        }
        return list;
    }
}