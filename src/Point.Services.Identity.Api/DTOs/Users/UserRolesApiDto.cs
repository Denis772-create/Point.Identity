﻿namespace Point.Services.Identity.Web.DTOs.Users;

public class UserRolesApiDto<TRoleDto>
{
    public UserRolesApiDto()
    {
        Roles = new List<TRoleDto>();
    }

    public List<TRoleDto> Roles { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
}