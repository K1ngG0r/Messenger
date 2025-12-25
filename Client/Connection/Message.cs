namespace Shared;

public sealed record Request(
    RequestMethod Method,
    string Body);

public sealed record Response(
    ResponseStatusCode Code,
    string Body);

public enum RequestMethod
{
    Time = 0
}
public enum ResponseStatusCode
{
    Ok = 0,
    Failed = 1
}