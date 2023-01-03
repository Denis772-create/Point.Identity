namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IUserClaimDto : IBaseUserClaimDto
{
    string ClaimType { get; set; }
    string ClaimValue { get; set; }
}