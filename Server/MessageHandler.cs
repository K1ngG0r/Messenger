using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Shared;
using SQLitePCL;

namespace Server;

public class MessageHandler
{
    private ApplicationDbContext _context;
    private SessionManager _sessionManager;

    public MessageHandler()
    {
        _context = new ApplicationDbContext();

        _sessionManager = new SessionManager(_context);

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
        var request = JsonSerializer.Deserialize<Request>(requestString);

        var correlationId = request!.CorrelationId;

        Response response = request!.Method switch
        {
            RequestMethod.Login => Login(correlationId, request.Body),
            RequestMethod.Send => Send(correlationId, request.SessionKey, request.Body),
            RequestMethod.Update => Update(correlationId, request.SessionKey, request.Body),
            RequestMethod.CreateChat => CreateChat(correlationId, request.SessionKey, request.Body),
            RequestMethod.Load => Load(correlationId, request.SessionKey, request.Body),
            _ => new Response(correlationId, ResponseStatusCode.Failed, "Команда не распознана")
        };
        return JsonSerializer.Serialize(response);
    }

    private Response CreateChat(Guid correlationId, string sessionKey, string username)
    {
        var user = _context.Users.FirstOrDefault(x => x.UserName == username);

        if(user == null)
            return new Response(correlationId, ResponseStatusCode.NotFound, string.Empty);

        var newChat = _context.Chats.Add(new Chat()
        {
            Members = new List<User>()
            {
                _sessionManager.GetUserBySession(sessionKey)!,
                user
            }
        }).Entity;

        return new Response(correlationId, ResponseStatusCode.Ok, 
            JsonSerializer.Serialize(newChat.Id));

    }

    private Response Update(Guid correlationId, string sessionKey ,string SendSettings)
    {
        var user = _sessionManager.GetUserBySession(sessionKey);

        var updates = _context.Users.First(x => x.Id == user!.Id).UnreadUpdate;

        return new Response(correlationId, ResponseStatusCode.Ok, 
                JsonSerializer.Serialize(updates));
    }

    private Response Send(Guid correlationId, string senderSessionKey, string SendSettings)
    {
        var settings = JsonSerializer.Deserialize<SendRequestSettings>(SendSettings)!;

        var chat = _context.Chats.First(x => x.Id == settings.chatId);

        if(chat != null)
            foreach(var u in chat.Members!)
            {
                var msg = new Message
                {
                    MessageText = settings.message,
                    ChatId = chat.Id,
                    SenderUserName = _sessionManager.GetUserBySession(senderSessionKey)!.UserName
                };

                var changes = new SingleChange(u.UnreadUpdate!.Count + 1, SingleChangeMethod.NewMessage, 
                    JsonSerializer.Serialize(msg));

                u.UnreadUpdate!.Add(changes);
            }
        else 
            return new Response(correlationId, ResponseStatusCode.NotFound, String.Empty);

        return new Response(correlationId, ResponseStatusCode.Ok, String.Empty);
    }
    private Response Load(Guid correlationId, string sessionKey, string body)
    {
        var settings = JsonSerializer.Deserialize<LoadRequestSettings>(body)!;

        switch (settings.method)
        {
            case LoadRequestSettingsMethod.User:
                var user = _context.Users.FirstOrDefault(x => x.UserName == body);
                if (user is null)
                    return new Response(correlationId, ResponseStatusCode.NotFound, "Не удалось найти");
                return new Response(correlationId, ResponseStatusCode.Ok, string.Empty);
            case LoadRequestSettingsMethod.Chat:
                if (Guid.TryParse(body, out Guid chatId))
                    break;
                var chat = _context.Chats.FirstOrDefault(x => x.Id == chatId);
                if(chat is null)
                    return new Response(correlationId, ResponseStatusCode.NotFound, "Не удалось найти");

                var chatInfoString = string.Empty;
                switch (chat.type)
                {
                    case CreateChatRequestSettingsMethod.PrivateChat:
                        var correspondentUsername = chat.Members?.FirstOrDefault(
                                x => x.UserName != _sessionManager
                                .GetUserBySession(sessionKey)?.UserName)?.UserName;
                        if(correspondentUsername is null)
                            return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");

                        chatInfoString = JsonSerializer.Serialize(
                            new PrivateChatInfo(correspondentUsername));
                        break;
                    case CreateChatRequestSettingsMethod.GroupChat:
                        break;//fixit
                }
                if(chatInfoString != string.Empty)
                    return new Response(correlationId, ResponseStatusCode.Ok, chatInfoString);
                return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");
        }
        return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");
    }

    private Response Login(Guid correlationId, string LoginSettings)
    {
        var settings = JsonSerializer.Deserialize<LoginRequestSettings>(LoginSettings)!;

        string password = hashPassword(settings.password);
        var res = _context.Users.FirstOrDefault(x => x.UserName == settings.username);

        var tmpUser = new User();
        if(res == null)
        {
            tmpUser = new User()
            {
                UserName = settings.username,
                HashPassword = password
            };
            _context.Users.Add(tmpUser);
        }
        else
        {
            if(password != res!.HashPassword)
                return new Response(correlationId, ResponseStatusCode.Failed, string.Empty);

            tmpUser = res;
        }

        string sessionKey = _sessionManager.CreateSession(tmpUser.Id);
        return new Response(correlationId, ResponseStatusCode.Ok, sessionKey);
    }

    private string hashPassword(string password)
    {
        MD5 md5 = MD5.Create();

        byte[] b = Encoding.ASCII.GetBytes(password);
        byte[] hash = md5.ComputeHash(b);

        StringBuilder sb = new StringBuilder();
        foreach(var a in hash)
            sb.Append(a.ToString("X2"));

        return Convert.ToString(sb)!;
    }
}
