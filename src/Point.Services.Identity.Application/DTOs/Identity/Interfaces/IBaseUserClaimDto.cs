namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IBaseUserClaimDto
{
    int ClaimId { get; set; }
    object UserId { get; }
}