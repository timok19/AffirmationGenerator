using System.Text.Json.Serialization;

namespace AffirmationGenerator.Server.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AffirmationLanguage
{
    English = 0,
    German = 1,
    Czech = 2,
    French = 3,
    Russian = 4,
}
