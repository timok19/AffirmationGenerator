using AffirmationGenerator.Server.Core;
using NSubstitute;
using NSubstitute.Core;

namespace AffirmationGenerator.Server.Tests.Extensions;

public static class NSubstituteExtensions
{
    extension<T>(Task<Result<T>> value)
    {
        public ConfiguredCall ReturnsSuccess(T returnThis) => value.Returns(Result<T>.Success(returnThis));

        public ConfiguredCall ReturnsError<TError>()
            where TError : ErrorDetails, new() => value.Returns(Result<T>.Error(new TError()));
    }

    extension<T>(Result<T> value)
    {
        public ConfiguredCall ReturnsSuccess(T returnThis) => value.Returns(Result<T>.Success(returnThis));

        public ConfiguredCall ReturnsError<TError>()
            where TError : ErrorDetails, new() => value.Returns(Result<T>.Error(new TError()));
    }
}
