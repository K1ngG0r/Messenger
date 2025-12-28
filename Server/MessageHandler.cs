using System.Text;
using System.Text.Json;
using Shared;

namespace Server;

internal static class MessageHandler
{
    static public string RequestHandler(byte[] requestBytes, CancellationToken cancellationToken)
    {
        var requestString = Encoding.UTF8.GetString(requestBytes);
        Console.WriteLine(requestString);
        var request = JsonSerializer.Deserialize<Request>(requestString);

        Response response = request.Method switch
        {
            RequestMethod.Send => SendMessage(),
            _ => new Response(ResponseStatusCode.Failed, "Команда не распознана")
        };

        return JsonSerializer.Serialize(response);
    }

    static private Response SendMessage()
    {
        

        return new Response(ResponseStatusCode.Ok, string.Empty);
    }
}