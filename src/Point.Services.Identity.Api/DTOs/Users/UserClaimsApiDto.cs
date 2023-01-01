﻿namespace Point.Services.Identity.Web.DTOs.Users;

public class UserClaimsApiDto<TKey>
{
    public UserClaimsApiDto()
    {
        Claims = new List<UserClaimApiDto<TKey>>();
    }

    public List<UserClaimApiDto<TKey>> Claims { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }
}