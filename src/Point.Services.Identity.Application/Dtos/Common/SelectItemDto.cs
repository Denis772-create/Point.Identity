﻿namespace Point.Services.Identity.Application.DTOs.Common;

public class SelectItemDto
{
    public SelectItemDto(string id, string text)
    {
        Id = id;
        Text = text;
    }

    public string Id { get; set; }

    public string Text { get; set; }
}