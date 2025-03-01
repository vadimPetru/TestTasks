/// <summary>
/// Represents an error with a code and message, supporting equality checks and string conversion.
/// </summary>
public class Error : IEquatable<Error>
{
    /// <summary>
    /// Represents the absence of an error (no error).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Represents an error for a null value.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with a specific code and message.
    /// </summary>
    /// <param name="code">The unique code identifying the error.</param>
    /// <param name="message">The description of the error.</param>
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
    /// <summary>
    /// Gets the unique code for the error.
    /// </summary>
    public string Code { get; }
    /// <summary>
    /// Gets the error message providing details about the error.
    /// </summary>
    public string Message { get; }
    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a <see cref="string"/> representation using the error code.
    /// </summary>
    /// <param name="error">The error to convert to a string.</param>
    public static implicit operator string(Error error) => error.Code;
    /// <summary>
    /// Checks if two <see cref="Error"/> instances are equal.
    /// </summary>
    /// <param name="a">The first error to compare.</param>
    /// <param name="b">The second error to compare.</param>
    /// <returns>True if both errors are equal; otherwise, false.</returns>
    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }
    /// <summary>
    /// Checks if two <see cref="Error"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first error to compare.</param>
    /// <param name="b">The second error to compare.</param>
    /// <returns>True if the errors are not equal; otherwise, false.</returns>
    public static bool operator !=(Error? a, Error? b) => !(a == b);
    /// <summary>
    /// Determines whether the specified <see cref="Error"/> is equal to the current <see cref="Error"/>.
    /// </summary>
    /// <param name="other">The other error to compare with the current error.</param>
    /// <returns>True if the errors are equal; otherwise, false.</returns>
    public virtual bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message;
    }
    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="Error"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current error.</param>
    /// <returns>True if the specified object is equal to the current error; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Error error && Equals(error);
    /// <summary>
    /// Gets the hash code for the current <see cref="Error"/> instance.
    /// </summary>
    /// <returns>The hash code for the error, based on its code and message.</returns>
    public override int GetHashCode() => HashCode.Combine(Code, Message);
    /// <summary>
    /// Returns the string representation of the error, which is the error code.
    /// </summary>
    /// <returns>The error code as a string.</returns>
    public override string ToString() => Code;
}