namespace Point.Services.Identity.Application.DTOs.Identity.Base;

public class BaseUserProviderDto<TUserId> : IBaseUserProviderDto
{
    public TUserId UserId { get; set; }

    object IBaseUserProviderDto.UserId => UserId;
}