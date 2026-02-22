namespace AffirmationGenerator.Server.Api;

public sealed record ApiOptions
{
    public required string ClientIpHeaderName { get; init; }

    public required int MaxRequestsPerDay { get; init; }
}
