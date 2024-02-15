namespace RainbowMatchTracker.Api.Middleware;

[ApiController]
public abstract class BaseController : ControllerBase
{
    [NonAction]
    public RequestValidator Validator() => new();

    [NonAction]
    public IActionResult Do(RequestResult result) => StatusCode((int)result.Code, result);

    [NonAction]
    public IActionResult DoOk() => Do(Requests.Ok());

    [NonAction]
    public IActionResult DoOk<T>(T data) => Do(Requests.Ok(data));

    [NonAction]
    public IActionResult DoNotFound(params string[] resources) => Do(Requests.NotFound(resources));

    [NonAction]
    public IActionResult DoBadRequest(params string[] issues) => Do(Requests.BadRequest(issues));

    [NonAction]
    public IActionResult DoUnauthorized(params string[] issues) => Do(Requests.Unauthorized(issues));

    [NonAction]
    public IActionResult DoCreated() => Do(Requests.Created());

    [NonAction]
    public IActionResult DoConflict(params string[] issues) => Do(Requests.Conflict(issues));

    [NonAction]
    public IActionResult DoException(params string[] issues) => Do(Requests.Exception(issues));

    [NonAction]
    public IActionResult DoException(params Exception[] errors) => Do(Requests.Exception(errors));

    [NonAction]
    public bool IsValid(RequestValidator validator, out IActionResult result)
    {
        var issues = validator.Issues;
        if (issues.Length == 0)
        {
            result = DoOk();
            return true;
        }

        result = DoBadRequest(validator.Issues);
        return false;
    }
}
