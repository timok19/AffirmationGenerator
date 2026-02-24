using System.Diagnostics.CodeAnalysis;
using AffirmationGenerator.Server.Core;
using Shouldly;

namespace AffirmationGenerator.Server.Tests.Extensions;

public static class ShouldyExtensions
{
    extension<T>([NotNull] Result<T>? actual)
    {
        public T ShouldBeSuccess()
        {
            actual.ShouldBeOfType<Success<T>>(GetErrorMessage(actual));
            return ((Success<T>)actual).Value;
        }

        public ErrorDetails ShouldBeError()
        {
            actual.ShouldBeOfType<Error<T>>();
            return ((Error<T>)actual).Details;
        }
    }

    private static string? GetErrorMessage<T>(Result<T>? actual)
    {
        var errorDetails = (actual as Error<T>)?.Details;
        return errorDetails?.GetType().Name;
    }
}
