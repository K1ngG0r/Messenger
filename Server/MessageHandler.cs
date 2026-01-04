using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Shared;

namespace Server;

public class MessageHandler
{
    public List<User> listUsers = new List<User>();

    public MessageHandler()
    {
        var localEndPoint = new IPEndPoint(IPAddress.Loopback, 9000);

        listUsers.Add(new User("1", "kinggor", localEndPoint));
        listUsers.Add(new User("2", "fazber", localEndPoint));
    } 

    public string RequestHandler(byte[] requestBytes, CancellationToken cancellationToken)
    {
        var requestString = Encoding.UTF8.GetString(requestBytes);
        Console.WriteLine(requestString);
        var request = JsonSerializer.Deserialize<Request>(requestString);

        Response response = request!.Method switch
        {
            RequestMethod.Send => SendMessage(request.Body),
            RequestMethod.Update => CheckMessage(request.Body),
            _ => new Response(ResponseStatusCode.Failed, "Команда не распознана")
        };

        return JsonSerializer.Serialize(response);
    }

    private Response SendMessage(string stringMessage)
    {
        Message message     = JsonSerializer.Deserialize<Message>(stringMessage)!;
        int userPosition    = listUsers.FindIndex(x => x.userName == message.receiverUserName);
        
        if(userPosition < 0)
        {
            return new Response(ResponseStatusCode.Failed, "Unknown user!");
        }

        listUsers[userPosition].unreadMessage?.Add(message);

        return new Response(ResponseStatusCode.Ok, string.Empty);
    }

    private Response CheckMessage(string senderKey)
    {
        int userPosition    = listUsers.FindIndex(x => x.key == senderKey);
        
        if(userPosition < 0)
        {
            return new Response(ResponseStatusCode.Failed, "Unknown key!");
        }

        string undeadMessageResponse = JsonSerializer.Serialize<List<Message>>(listUsers[userPosition].unreadMessage!);

        return new Response(ResponseStatusCode.Ok, undeadMessageResponse);
    }
}