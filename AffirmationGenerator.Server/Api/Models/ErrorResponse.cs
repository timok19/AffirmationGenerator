namespace AffirmationGenerator.Server.Api.Models;

public record ErrorResponse
{
    public required string Details { get; init; }
}
