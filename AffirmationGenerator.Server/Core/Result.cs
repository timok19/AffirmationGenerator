namespace AffirmationGenerator.Server.Core;

public abstract record Result<T>
{
    public static Result<Unit> Success() => Result<Unit>.Success(new Unit());

    public static Result<T> Success(T value) => new Success<T>(value);

    public static Result<T> Error(ErrorDetails errorDetails) => new Error<T>(errorDetails);

    public static implicit operator Result<T>(T value) => new Success<T>(value);
}

public record Success<T>(T Value) : Result<T>;

public record Error<T>(ErrorDetails Details) : Result<T>;

public abstract record ErrorDetails;
