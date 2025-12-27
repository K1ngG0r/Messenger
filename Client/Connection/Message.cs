namespace Client.Connection;

public sealed record Request(
    RequestMethod Method,
    string Body);

public sealed record Response(
    ResponseStatusCode Code,
    string Body);

public enum RequestMethod
{
    Register,
    Login,
    Send,
    Update,
    Load
}
public enum ResponseStatusCode
{
    Ok,
    Failed
}