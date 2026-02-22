namespace AffirmationGenerator.Server;

public sealed record ServerOptions
{
    public required string ClientIpHeaderName { get; init; }
}
