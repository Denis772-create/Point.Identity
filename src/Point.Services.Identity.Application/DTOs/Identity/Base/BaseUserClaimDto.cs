namespace Point.Services.Identity.Application.DTOs.Identity.Base;

public class BaseUserClaimDto<TUserId> : IBaseUserClaimDto
{
    public int ClaimId { get; set; }

    public TUserId UserId { get; set; }

    object IBaseUserClaimDto.UserId => UserId;
}