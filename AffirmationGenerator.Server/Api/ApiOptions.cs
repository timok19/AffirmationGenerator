namespace AffirmationGenerator.Server.Api;

public sealed record ApiOptions
{
    public required string ClientIpHeaderName { get; init; }
}
