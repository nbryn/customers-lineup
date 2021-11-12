using System;
using System.Threading.Tasks;
using FluentValidation;

namespace CLup.Application.Shared
{
    public class Result
    {
        public bool Success { get; private set; }
        public string Error { get; private set; }
        public HttpCode Code { get; private set; }
        public bool Failure => !Success;

        protected Result(bool success, string error, HttpCode code)
        {
            Success = success;
            Error = error;
            Code = code;
        }

        public async Task<Result<T>> Bind<T>(Func<Task<T>> f, string errorMessage)
        {
            var maybe = await f();

            if (maybe == null)
            {
                return NotFound<T>(errorMessage);
            }

            return Success ? Ok(maybe) : Fail<T>(Code, Error);
        }

        public async Task<Result<T>> Bind<T>(Func<Task<T>> f)
        {
            if (Failure)
            {
                return Fail<T>(Code, Error);
            }

            var maybe = await f();

            return Ok<T>(maybe);
        }

        public async Task<Result> BindIgnore<T>(Func<Task<T>> f, string errorMessage)
        {
            if (Success)
            {
                var maybe = await f();

                return maybe == null ? NotFound(errorMessage) : Ok();
            }

            return Fail(Code, Error);
        }

        public Result<T> Bind<T>(Func<T> f) => Success ? Ok<T>(f()) : Fail<T>(Code, Error);

        public static Result<T> ToResult<T>(T maybe, string errorMessage) => maybe == null ? NotFound<T>(errorMessage) : Ok<T>(maybe);

        public static Result ToResultIgnore<T>(T maybe, string errorMessage) => maybe == null ? NotFound(errorMessage) : Ok();

        public static Result Ok() => new(true, String.Empty, HttpCode.Ok);

        public static Result<T> Ok<T>(T value) => new(value, true, String.Empty, HttpCode.Ok);

        public static Result<T, U> Ok<T, U>(T value, U extraValue) => new(value, extraValue, true, String.Empty, HttpCode.Ok);

        public static Result Fail(HttpCode code, string message) => new(false, message, code);

        public static Result<T> Fail<T>(HttpCode code, string message) => new(default, false, message, code);

        public static Result<T, U> Fail<T, U>(HttpCode code, string message) => new(default, default, false, message, code);

        public static Result NotFound(string message = "") => new(false, message, HttpCode.NotFound);

        public static Result<T> NotFound<T>(string message = "") => new(default, false, message, HttpCode.NotFound);

        public static Result Forbidden(string message) => new(false, message, HttpCode.Forbidden);

        public static Result Conflict(string message = "") => new(false, message, HttpCode.Conflict);

        public static Result<T> Conflict<T>(string message = "") => new(default, false, message, HttpCode.Conflict);

        public static Result<T> BadRequest<T>(string message = "") => new(default, false, message, HttpCode.BadRequest);

        public static Result<T> Unauthorized<T>() => new(default(T), false, String.Empty, HttpCode.Unauthorized);
    }
    public class Result<T> : Result
    {
        public T Value { get; set; }

        protected internal Result(T value, bool success, string error, HttpCode code)
            : base(success, error, code)
        {
            Value = value;
        }

        public Result<U> Bind<U>(Func<T, U> f) => Success ? Ok<U>(f(Value)) : Fail<U>(Code, Error);

        public Result<T> AddDomainEvent(Action<T> f)
        {
            if (Failure)
            {
                return Fail<T>(Code, Error);
            }

            f(Value);

            return Ok(Value);
        }

        public async Task<Result<U>> Bind<U>(Func<T, Task<U>> f)
        {
            if (Failure)
            {
                return Fail<U>(Code, Error);
            }

            var maybe = await f(Value);

            return Ok(maybe);
        }

        public async Task<Result<T>> BindF<U>(Func<T, Task<U>> f)
        {
            if (Failure)
            {
                return Fail<T>(Code, Error);
            }

            await f(Value);

            return Ok(Value);
        }

        public async Task<Result<T>> BindF(Func<T, Task> f)
        {
            if (Failure)
            {
                return Fail<T>(Code, Error);
            }

            await f(Value);

            return Ok(Value);
        }

        public Result BindIgnore(Action<T> f)
        {
            if (Success)
            {
                f(Value);

                return Ok();
            }

            return Fail(Code, Error);
        }

        public async Task<Result<T>> Ensure(Task<Result<T>> task, Func<T, bool> predicate, (HttpCode code, string message) errorInfo)
        {
            if (Failure)
            {
                return Fail<T>(Code, Error);
            }

            if (!predicate(Value))
            {
                return Fail<T>(errorInfo.code, errorInfo.message);
            }

            return await task;
        }

        public Result EnsureIgnore(Func<T, bool> predicate, string errorMessage)
        {
            if (Failure)
            {
                return Fail(Code, Error);
            }

            return !predicate(Value) ? Conflict(errorMessage) : Ok();
        }

        public Result EnsureIgnore(Func<T, bool> predicate, (HttpCode code, string message) errorInfo)
        {
            if (Failure)
            {
                return Fail(Code, Error);
            }

            return !predicate(Value) ? Fail(errorInfo.code, errorInfo.message) : Ok();
        }

        public async Task<Result<T, U>> BindDouble<U>(Func<Task<U>> f)
        {
            if (Failure)
            {
                return Fail<T, U>(Code, Error);
            }

            var maybe = await f();

            return Ok(Value, maybe);
        }

        public Result<T, U> BindDouble<U>(Func<T, U> f)
        {
            if (Failure)
            {
                return Fail<T, U>(Code, Error);
            }

            var maybe = f(Value);

            return Ok(Value, maybe);
        }


        public Result<T> Validate(IValidator<T> validator)
        {
            if (Failure)
            {
                return Fail<T>(Code, Error);
            }

            var validationResult = validator.Validate(Value);
            return !validationResult.IsValid ? BadRequest<T>(validationResult.Errors[0].ErrorMessage) : Ok(Value);
        }
    }

    public class Result<T, U> : Result<T>
    {
        public U ExtraValue { get; set; }

        protected internal Result(T value, U extraValue, bool success, string error, HttpCode code)
            : base(value, success, error, code)
        {
            ExtraValue = extraValue;
        }

        public Result<V> Bind<V>(Func<T, U, V> f)
        {
            if (Failure)
            {
                return Fail<V>(Code, Error);
            }

            var maybe = f(Value, ExtraValue);

            return Ok(maybe);
        }

        public async Task<Result<T>> Ensure(Task<Result<T, U>> task, Func<U, bool> predicate, string errorMessage)
        {
            if (Failure)
            {
                return Fail<T>(Code, Error);
            }

            if (!predicate(ExtraValue))
            {
                return Conflict<T>(errorMessage);
            }

            return await task;
        }
    }
}