using CLup.Domain.Shared;
using FluentValidation;
using NuGet.Packaging;

namespace CLup.Application.Shared;

public class Result : DomainResult
{
    public HttpCode Code { get; protected set; }

    protected Result(HttpCode code, IList<Error> errors) : base(errors)
    {
        Code = code;
    }

    public static new Result Ok() => new(HttpCode.Ok, []);

    public static Result<T> Ok<T>(T value) => new(value, HttpCode.Ok, []);

    public static Result BadRequest(IList<Error> errors) => Fail(HttpCode.BadRequest, errors);

    public static Result<T> BadRequest<T>(IList<Error> errors) => new(default, HttpCode.BadRequest, errors);

    public static Result Fail(HttpCode code, IList<Error> errors) => new(code, errors);

    public static Result<T> Fail<T>(HttpCode code, IList<Error> errors, T value = default) => new(value, code, errors);

    public static Result<T> NotFound<T>(IList<Error> errors) => new(default, HttpCode.NotFound, errors);

    public static Result<T> ToResult<T>(T? value, Error? error = null)
        => value is null ? NotFound<T>([error]) : Ok(value);

    public static Result Validate<TRequest, TValidator>(TRequest request) where TValidator : AbstractValidator<TRequest>, new()
    {
        var validationResult = new TValidator().Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error => new Error(error.PropertyName, error.ErrorMessage));
            return BadRequest(errors.ToList());
        }

        return Ok();
    }

    public Result<T> Combine<T>(T? value, IList<Error>? errors = null)
    {
        if (Failure)
        {
            return BadRequest<T>(Errors);
        }

        if (value is null)
        {
            return BadRequest<T>(errors ?? []);
        }

        return Ok(value);
    }

    public async Task<Result> BindAsync<T>(Func<Task<T>> f)
    {
        await f();
        return this;
    }

    public async Task<Result> FlatMap(Task<Result<DomainResult>> task)
    {
        var result = await task;
        if (result.Value?.Failure ?? false)
        {
            Code = HttpCode.BadRequest;
            Errors.AddRange(result.Value.Errors);
        }

        return this;
    }

    public ProblemDetails ToProblemDetails()
    {
        if (Success)
        {
            throw new InvalidOperationException("Can't convert result to problem details.");
        }

        return new ProblemDetails(
            Code,
            Errors
                .GroupBy(error => error.Code)
                .ToDictionary(group => group.Key, group => group.Select(item => item.Message).ToList()));
    }
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    internal Result(T value, HttpCode code, IList<Error> errors)
        : base(code, errors)
    {
        Value = value;
    }

    public Result<U> Bind<U>(Func<T, U> f)
    {
        if (Failure)
        {
            return Fail<U>(Code, Errors);
        }

        var value = f(Value);

        return Ok(value);
    }

    public Result<T> AddDomainEvent(Action<T> f)
    {
        if (Failure)
        {
            return this;
        }

        f(Value);

        return Ok(Value);
    }

    public async Task<Result<U>> BindAsync<U>(Func<T, Task<U>> f)
    {
        if (Failure)
        {
            return Fail<U>(Code, Errors);
        }

        var maybe = await f(Value);
        if (Failure)
        {
            return Fail(Code, Errors, maybe);
        }

        return Ok(maybe);
    }

    public Result<U> Bind<U>(Func<T, U> f, Error error)
    {
        if (Failure)
        {
            return Fail<U>(HttpCode.NotFound, Errors);
        }

        var maybe = f(Value);
        if (maybe == null)
        {
            Errors.Add(error);
            return Fail(HttpCode.NotFound, Errors, maybe);
        }

        return Ok(maybe);
    }

    public async Task<Result<U>> FailureIfNotFoundAsync<U>(Func<T, Task<U>> f, Error error)
    {
        if (Failure)
        {
            return Fail<U>(Code, Errors);
        }

        var maybe = await f(Value);
        if (maybe == null)
        {
            Errors.Add(error);
            return Fail(HttpCode.NotFound, Errors, maybe);
        }

        return Ok(maybe);
    }

    public async Task<Result<T>> BindF<U>(Func<T, Task<U>> f)
    {
        if (Value == null)
        {
            return this;
        }

        await f(Value);
        return this;
    }

    public async Task<Result<T>> Ensure(
        Task<Result<T>> task,
        Func<T, bool> predicate,
        HttpCode httpCode,
        Error? error = null)
    {
        await task;
        if (Failure)
        {
            return this;
        }

        if (!predicate(Value))
        {
            Code = httpCode;
            if (error != null)
            {
                Errors.Add(error);
            }
        }

        return this;
    }

    public async Task<Result<T>> FlatMap(
        Task<Result<T>> task,
        Func<T, DomainResult> f,
        HttpCode httpCode)
    {
        await task;
        if (Failure)
        {
            return this;
        }

        var result = f(Value);
        if (result.Failure)
        {
            Code = httpCode;
            Errors.AddRange(result.Errors);
        }

        return this;
    }

    public Result<T> Validate(IValidator<T> validator)
    {
        if (Value == null)
        {
            return this;
        }

        var validationResult = validator.Validate(Value);
        if (Failure || !validationResult.IsValid)
        {
            var validationErrors =
                validationResult?.Errors.Select(error => new Error(error.PropertyName, error.ErrorMessage)) ?? [];

            Errors.AddRange(validationErrors);
            Code = HttpCode.BadRequest;
        }

        return this;
    }
}
