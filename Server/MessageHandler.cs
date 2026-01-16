using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Shared;

namespace Server;

public class MessageHandler
{
    private ApplicationDbContext _context = new ApplicationDbContext();

    public MessageHandler()
    {
        
        // Создаем БД если не существует
        _context.Database.EnsureCreated();

        // _context.Users.Add(new User(){UserName = "kinggor"});
        // _context.Users.Add(new User(){UserName = "fazber"});
        // _context.SaveChanges();

        // Загружаем пользователей ИЗ БД
        var _users = _context.Users.ToDictionary(x => x.Id, x => x);
        
        Console.WriteLine($"Загружено {_users.Count} пользователей из БД");

    } 

    public string RequestHandler(byte[] requestBytes, CancellationToken cancellationToken)
    {
        var requestString = Encoding.UTF8.GetString(requestBytes);
        Console.WriteLine(requestString);
        var request = JsonSerializer.Deserialize<Request>(requestString);

        var correlationId = request!.CorrelationId;

        Response response = request!.Method switch
        {
            RequestMethod.Login => Login(correlationId, request.Body),
            _ => new Response(correlationId, ResponseStatusCode.Failed, "Команда не распознана")
        };

        return JsonSerializer.Serialize(response);
    }

    private Response Login(Guid correlationId ,string stringMessage)
    {
        Message message     = JsonSerializer.Deserialize<Message>(stringMessage)!;
        

        return new Response(correlationId, ResponseStatusCode.Ok, string.Empty);
    }
}