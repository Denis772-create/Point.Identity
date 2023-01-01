namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IUsersDto
{
    int PageSize { get; set; }
    int TotalCount { get; set; }
    List<IUserDto> Users { get; }
}