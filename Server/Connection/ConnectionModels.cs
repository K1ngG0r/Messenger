using System.Text.Json.Serialization;

namespace Server;

//Login
public sealed record LoginRequestSettings(
    string username,
    string password);
public sealed record LoginResponseSettings(string sessionKey,
    string name,
    byte[] avatar);//другие данные пользователя (приватные,
                   //например черный список, который другие пользователи видеть не могут)

//Send
public sealed record SendRequestSettings(
    Guid chatId,
    string message);
//response - string.empty

//Update
//request - string.empty
//response - List<SingleChange>
public sealed record SingleChange(
    int Id,
    SingleChangeMethod method,
    string body);
public enum SingleChangeMethod
{
    NewMessage,
    NewChat,
    ParticipantAction//удаление из чата, например,
                     //или назначение админом,
                     //добавление меня в группу
                     //и проч (изменение ника пользователя, аватарки)
}
//ParticipantAction
public sealed record ParticipantActionRequestSettings(
    ParticipantActionRequestSettingsMethod method,
    Guid chatId,
    string body);
public enum ParticipantActionRequestSettingsMethod
{
    AddParticipant, //body - username
    RemoveParticipant, //body - username
    Op,//сделать админом //body - username
       //можно список прав еще:
       //возможность удалять сообщения, назначать админом других
    Leave,
    Deop//убрать права админа //body - username
    //и тд
}
//response - string.empty

//CreateChat
public sealed record CreateChatRequestSettings(
    CreateChatRequestSettingsMethod method,
    string body);
public enum CreateChatRequestSettingsMethod
{
    PrivateChat, //body - username
    GroupChat, //body - chatname
    ChannelChat //body - chatname
                //может для группы и канала
                //еще ограничение по макс участникам и др
                //(для этого отдельная структура)
}
//response - guid чата

//Load
public sealed record LoadRequestSettings(
    LoadRequestSettingsMethod method,
    string body);
public enum LoadRequestSettingsMethod
{
    User,//body - username
    Chat//body - guid chatid
}
public sealed record UserInfo(string name,
        string username,
        byte[] avatar);
public sealed record PrivateChatInfo(string username);
public sealed record GroupChatInfo(string chatName,
    List<ParticipantInfo> participants,
    string ownerUsername);
public sealed record ChannelChatInfo(string chatName,
    List<ParticipantInfo> subscribers,
    string ownerUsername);
public sealed record ParticipantInfo(string username,
    ParticipantInfoType type);
public enum ParticipantInfoType
{
    Member,
    Admin
}

//ChangeSettings
public sealed record ChangeSettingsRequestSettings(
    string username,
    string name,
    byte[] avatar//и проч (аватарка, дата рождения, статус)
    );
//response - string.empty