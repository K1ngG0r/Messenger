using System.Text.Json.Serialization;

namespace Client.Connection;

public sealed record Request(
    [property: JsonPropertyName("key")] string SessionKey,
    [property: JsonPropertyName("cid")] Guid CorrelationId,
    [property: JsonPropertyName("method")] RequestMethod Method,
    [property: JsonPropertyName("body")] string Body);

public sealed record Response(
    [property: JsonPropertyName("cid")] Guid CorrelationId,
    [property: JsonPropertyName("code")] ResponseStatusCode Code,
    [property: JsonPropertyName("body")] string Payload);

public enum RequestMethod
{
    Register,
    Login,
    Send,
    Update,
    AdminAction,
    CreateChat,
    LoadAvatar
}
public enum ResponseStatusCode
{
    Ok,
    NotFound,
    Failed
}