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
        //var _users = _context.Users.ToDictionary(x => x.Id, x => x);
        var _users = _context.Users.ToList();
        Console.WriteLine("Пользователи в системе:");
        Console.WriteLine(string.Join("\n", _users.Select(x=>x.UserName)));
        //Console.WriteLine($"Загружено {_users.Count} пользователей из БД");
    } 

    public string RequestHandler(byte[] requestBytes, CancellationToken cancellationToken)
    {
        var requestString = Encoding.UTF8.GetString(requestBytes);
        Console.WriteLine($"Попытка десериализовать запрос");
        var request = JsonSerializer.Deserialize<Request>(requestString);

        var correlationId = request!.CorrelationId;
        Console.WriteLine($"{request.SessionKey} >> Запрос десериализован.");

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

    private Response CreateChat(Guid correlationId, string sessionKey, string body)
    {
        Console.WriteLine("Запрос create chat");
        CreateChatRequestSettings? settings = JsonSerializer.Deserialize<CreateChatRequestSettings?>(body);
        if(settings is null)
            return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");

        switch (settings.method)
        {
            case CreateChatRequestSettingsMethod.PrivateChat:
                Console.WriteLine("Запрос create chat private");
                var user = _context.Users.FirstOrDefault(x => x.UserName == settings.body);

                if (user == null)
                    return new Response(correlationId, ResponseStatusCode.NotFound, string.Empty);
                Console.WriteLine("user found");

                var newChat = new Chat()
                {

                    type = CreateChatRequestSettingsMethod.PrivateChat,
                    Members = new List<User>()
                    {
                        _sessionManager.GetUserBySession(sessionKey)!,
                        user
                    }
                };
                newChat = _context.Chats.Add(newChat).Entity;
                Console.WriteLine("add new chat");
                try
                {
                    _context.SaveChanges();
                    Console.WriteLine("save changes all right");
                    return new Response(correlationId, ResponseStatusCode.Ok,
                        newChat.Id.ToString());
                }
                catch(Exception e)
                {

                    Console.WriteLine($"err savch {e.Message}");
                }
                break;
        }
        Console.WriteLine("Чат не создан");
        return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");
    }

    private Response Update(Guid correlationId, string sessionKey ,string SendSettings)
    {
        Console.WriteLine("Запрос update");
        var user = _sessionManager.GetUserBySession(sessionKey);

        var updates = _context.Users.First(x => x.Id == user!.Id).UnreadUpdate;

        Console.WriteLine("Запрос update успешно отработан");
        return new Response(correlationId, ResponseStatusCode.Ok, 
                JsonSerializer.Serialize(updates));
    }

    private Response Send(Guid correlationId, string senderSessionKey, string SendSettings)
    {
        Console.WriteLine("Запрос send");
        var settings = JsonSerializer.Deserialize<SendRequestSettings>(SendSettings);
        if(settings is null)
            return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");

        try
        {
            var chat = _context.Chats.FirstOrDefault(x => x.Id == settings.chatId); 
            if (chat is null)
                return new Response(correlationId, ResponseStatusCode.NotFound, String.Empty);

            foreach (var u in chat.Members!)
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
            Console.WriteLine("Запрос send успешно отработан");
            return new Response(correlationId, ResponseStatusCode.Ok, String.Empty);
        }
        catch
        {
            return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");
        }

        
    }
    private Response Load(Guid correlationId, string sessionKey, string body)
    {
        Console.WriteLine("Запрос load");
        var settings = JsonSerializer.Deserialize<LoadRequestSettings>(body)!;

        switch (settings.method)
        {
            case LoadRequestSettingsMethod.User:
                Console.WriteLine("Запрос load user");
                var user = _context.Users.FirstOrDefault(x => x.UserName == settings.body);
                if (user is null)
                {
                    return new Response(correlationId, ResponseStatusCode.NotFound, "Не удалось найти");
                }
                var userInfoString = JsonSerializer.Serialize(new UserInfo("Daniil", user.UserName, new byte[32]));
                Console.WriteLine("Запрос load user успешно отработан");
                return new Response(correlationId, ResponseStatusCode.Ok, userInfoString);
            case LoadRequestSettingsMethod.Chat:
                Console.WriteLine("Запрос load chat");
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
                Console.WriteLine("Запрос update - ошибка");
                return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");
        }
        return new Response(correlationId, ResponseStatusCode.Failed, "Ошибка");
    }

    private Response Login(Guid correlationId, string LoginSettings)
    {
        Console.WriteLine("Запрос login");
        var settings = JsonSerializer.Deserialize<LoginRequestSettings>(LoginSettings)!;

        string password = hashPassword(settings.password);
        var user = _context.Users.FirstOrDefault(x => x.UserName == settings.username);

        var tmpUser = new User();
        if(user == null)
        {
            tmpUser = new User()
            {

                UserName = settings.username,
                HashPassword = password
            };
            _context.Users.Add(tmpUser);
            _context.SaveChanges();
        }
        else
        {
            if(password != user!.HashPassword)
                return new Response(correlationId, ResponseStatusCode.Failed, "Неверный пароль");

            tmpUser = user;
        }

        string sessionKey = _sessionManager.CreateSession(tmpUser.Id);
        UserSettings userSettings = new UserSettings(tmpUser.UserName, "User", new byte[0]);
        List<Guid> userChats = _context.Chats.Where(
            x=>x.Members.FirstOrDefault(x=>x.UserName==tmpUser.UserName) != null)
            .Select(x=>x.Id)
            .ToList();
        var loginReqponseSettingsString = JsonSerializer.Serialize(
            new LoginResponseSettings(sessionKey, userSettings, userChats));
        Console.WriteLine("Запрос login успушно отработан");
        Console.WriteLine($"Пользователей {string.Join("/n", _context.Users)}");
        return new Response(correlationId, ResponseStatusCode.Ok, loginReqponseSettingsString);
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
