using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestTask.Models.ResultObject;
/// <summary>
/// Represents the outcome of an operation, with success or failure state and an optional error message.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with the specified success state and error.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation succeeded.</param>
    /// <param name="error">An error instance if the operation failed; otherwise, <see cref="Error.None"/>.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the success state and error conflict (e.g., success with an error or failure without an error).
    /// </exception>
    protected internal Result(bool isSuccess, Error error)
    {
        // Validate that success must correspond to a lack of error, and failure to the presence of an error
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }
    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;
    /// <summary>
    /// Gets the error associated with a failed operation; returns <see cref="Error.None"/> if the operation was successful.
    /// </summary>
    public Error Error { get; }
    /// <summary>
    /// Creates a successful result with no associated error.
    /// </summary>
    /// <returns>A new <see cref="Result"/> instance representing success.</returns>
    public static Result Success() => new(true, Error.None);
    /// <summary>
    /// Creates a successful result with an associated value of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value associated with the successful result.</typeparam>
    /// <param name="value">The value to associate with the success result.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a successful operation with a value.</returns>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error describing why the operation failed.</param>
    /// <returns>A new <see cref="Result"/> instance representing failure.</returns>
    public static Result Failure(Error error) => new(false, error);
    /// <summary>
    /// Creates a failed result with the specified error for a result type containing a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value associated with the result.</typeparam>
    /// <param name="error">The error describing why the operation failed.</param>
    /// <returns>A new <see cref="Result{TValue}"/> instance representing failure with a specified error.</returns>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> based on whether the specified value is null.
    /// If the value is non-null, a successful result is created; otherwise, a failure result with <see cref="Error.NullValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <param name="value">The value to check and wrap in a result.</param>
    /// <returns>
    /// A success result containing the value if it is not null; otherwise, a failure result with <see cref="Error.NullValue"/>.
    /// </returns>
    public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
