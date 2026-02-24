namespace AffirmationGenerator.Server.Application;

public sealed record ClientOptions
{
    public required string ClientIpHeaderName { get; init; }

    public required int MaxRequestsPerDay { get; init; }
}
