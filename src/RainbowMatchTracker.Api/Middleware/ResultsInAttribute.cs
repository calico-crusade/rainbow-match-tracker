namespace RainbowMatchTracker.Api.Middleware;

/// <summary>
/// Wraps the <see cref="ProducesResponseTypeAttribute"/> for the <see cref="RequestResult"/> types
/// </summary>
/// <param name="code">The optional status code of the result</param>
public class ResultsInAttribute(int code = 200)
    : ProducesResponseTypeAttribute(typeof(RequestResult), code)
{ }

/// <summary>
/// Wraps the <see cref="ProducesResponseTypeAttribute"/> for the <see cref="RequestResult{T}"/> types
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="code">The optional status code of the result</param>
public class ResultsInAttribute<T>(int code = 200)
    : ProducesResponseTypeAttribute(typeof(RequestResult<T>), code)
{ }

/// <summary>
/// Wraps the <see cref="ProducesResponseTypeAttribute"/> for the <see cref="ExceptionResult"/> types
/// </summary>
/// <param name="code">The optional status code of the result</param>
public class ResultsInErrorAttribute(int code = 500)
    : ProducesResponseTypeAttribute(typeof(ExceptionResult), code)
{ }
