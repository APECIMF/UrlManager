﻿using System.Net;

namespace TheGnouCommunity.UrlManager.Domain.Messaging;

public record RedirectionRequestFailed
{
    public required string Host { get; init; }
    public required string Path { get; init; }
    public string? IPAddress { get; init; }
    public required DateTime RequestTime { get; init; }
    public required string[] Errors { get; init; }
}