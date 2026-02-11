namespace AffirmationGenerator.Server.Core.Extensions;

public static class ResultExtensions
{
    public static T GetOrDefault<T>(this Result<T> result, T defaultValue) => result.Match(success => success, _ => defaultValue);

    extension<TIn>(Task<Result<TIn>> result)
    {
        public async Task<TOut> Match<TOut>(Func<TIn, TOut> mapSuccess, Func<ErrorDetails, TOut> mapError) =>
            await result switch
            {
                Success<TIn> success => mapSuccess(success.Value),
                Error<TIn> error => mapError(error.Details),
                _ => throw new ArgumentOutOfRangeException(nameof(result)),
            };

        public async Task<TOut> Match<TOut>(Func<TIn, Task<TOut>> mapSuccess, Func<ErrorDetails, TOut> mapError) =>
            await result switch
            {
                Success<TIn> success => await mapSuccess(success.Value),
                Error<TIn> error => mapError(error.Details),
                _ => throw new ArgumentOutOfRangeException(nameof(result)),
            };
    }

    extension<TIn>(Result<TIn> result)
    {
        public TOut Match<TOut>(Func<TIn, TOut> mapSuccess, Func<ErrorDetails, TOut> mapError) =>
            result switch
            {
                Success<TIn> success => mapSuccess(success.Value),
                Error<TIn> error => mapError(error.Details),
                _ => throw new ArgumentOutOfRangeException(nameof(result)),
            };

        public async Task<TOut> Match<TOut>(Func<TIn, Task<TOut>> mapSuccess, Func<ErrorDetails, TOut> mapError) =>
            result switch
            {
                Success<TIn> success => await mapSuccess(success.Value),
                Error<TIn> error => mapError(error.Details),
                _ => throw new ArgumentOutOfRangeException(nameof(result)),
            };

        public async Task<Result<TOut>> Map<TOut>(Func<TIn, Task<TOut>> map) =>
            result switch
            {
                Success<TIn> success => await map(success.Value),
                Error<TIn> error => Result<TOut>.Error(error.Details),
                _ => throw new ArgumentOutOfRangeException(nameof(result)),
            };
    }

    extension<TIn>(Task<Result<TIn>> result)
    {
        public async Task<Result<TOut>> Map<TOut>(Func<TIn, Task<TOut>> map) =>
            await result.Match(async value => Result<TOut>.Success(await map(value)), Result<TOut>.Error);

        public async Task<Result<TOut>> Map<TOut>(Func<TIn, TOut> map) => await result.Match(value => map(value), Result<TOut>.Error);

        public async Task<Result<TOut>> Bind<TOut>(Func<TIn, Result<TOut>> map) => await result.Match(map, Result<TOut>.Error);

        public async Task<Result<TOut>> Bind<TOut>(Func<TIn, Task<Result<TOut>>> map) => await result.Match(map, Result<TOut>.Error);
    }

    extension<TIn>(Result<TIn> result)
    {
        public async Task<Result<TOut>> Bind<TOut>(Func<TIn, Task<Result<TOut>>> map) =>
            await result.Match(async value => await map(value), Result<TOut>.Error);

        public Result<TOut> Bind<TOut>(Func<TIn, Result<TOut>> map) => result.Match(map, Result<TOut>.Error);

        public async Task<Result<TOut>> SelectMany<TInner, TOut>(Func<TIn, Task<Result<TInner>>> bind, Func<TIn, TInner, TOut> project) =>
            await result.Match(
                async outer => await bind(outer).Match(inner => Result<TOut>.Success(project(outer, inner)), Result<TOut>.Error),
                Result<TOut>.Error
            );
    }

    extension<T>(Task<Result<T>> result)
    {
        public async Task<T> GetOrDefault(T defaultValue) => await result.Match(success => success, _ => defaultValue);

        public async Task<T> GetOrDefault(Func<T> defaultValue) => await result.Match(success => success, _ => defaultValue());
    }

    extension<TOuter>(Task<Result<TOuter>> result)
    {
        public async Task<Result<TOut>> SelectMany<TInner, TOut>(
            Func<TOuter, Task<Result<TInner>>> bind,
            Func<TOuter, TInner, TOut> project
        ) =>
            await result.Match(
                async innerSuccess => await bind(innerSuccess).Match(success => project(innerSuccess, success), Result<TOut>.Error),
                Result<TOut>.Error
            );

        public async Task<Result<TOut>> SelectMany<TInner, TOut>(
            Func<TOuter, Task<Result<TInner>>> bind,
            Func<TOuter, TInner, Task<TOut>> project
        ) =>
            await result.Match(
                async outer =>
                    await bind(outer).Match(async success => Result<TOut>.Success(await project(outer, success)), Result<TOut>.Error),
                Result<TOut>.Error
            );

        public async Task<Result<TOut>> SelectMany<TInner, TOut>(Func<TOuter, Result<TInner>> bind, Func<TOuter, TInner, TOut> project) =>
            await result.Match(
                innerSuccess => bind(innerSuccess).Match(success => project(innerSuccess, success), Result<TOut>.Error),
                Result<TOut>.Error
            );

        public async Task<Result<TOut>> SelectMany<TInner, TOut>(
            Func<TOuter, Result<TInner>> bind,
            Func<TOuter, TInner, Task<TOut>> project
        ) =>
            await result.Match(
                async outer =>
                    await bind(outer).Match(async success => Result<TOut>.Success(await project(outer, success)), Result<TOut>.Error),
                Result<TOut>.Error
            );
    }
}
