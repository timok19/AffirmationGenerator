using AffirmationGenerator.Server.Core;
using NSubstitute;
using NSubstitute.Core;

namespace AffirmationGenerator.Server.Tests.Extensions;

public static class NSubstituteExtensions
{
    public static ConfiguredCall ReturnsSuccess<T>(this Task<Result<T>> value, T returnThis) =>
        value.Returns(Result<T>.Success(returnThis));

    public static ConfiguredCall ReturnsError<TValue, TError>(this Task<Result<TValue>> value)
        where TError : ErrorDetails, new() => value.Returns(Result<TValue>.Error(new TError()));
}
