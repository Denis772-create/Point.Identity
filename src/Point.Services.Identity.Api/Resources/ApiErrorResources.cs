namespace Point.Services.Identity.Web.Resources;

public class ApiErrorResources : IApiErrorResources
{
    public virtual ApiError CannotSetId()
        => new()
        {
            Code = nameof(CannotSetId),
            Description = ApiErrorResource.CannotSetId
        };
}