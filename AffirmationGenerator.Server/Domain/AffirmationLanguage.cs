using System.Runtime.Serialization;

namespace AffirmationGenerator.Server.Domain;

public enum AffirmationLanguage
{
    [EnumMember(Value = "en")]
    English,

    [EnumMember(Value = "de")]
    German,

    [EnumMember(Value = "cz")]
    Czech,
}
