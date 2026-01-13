using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Connection
{
    //Register/Login
    public sealed record LoginRequestSettings(
        string username,
        string password);
    //response - string sessionkey

    //Send
    public sealed record SendRequestSettings(
        Guid chatId,
        string message);
    //response - string.empty

    //Update
    //request - string.empty
    //response - List<SingleChange>
    public sealed record SingleChange(
        SingleChangeMethod method,
        string body);
    public enum SingleChangeMethod
    {
        NewMessage,
        NewChat,
        AdminAction//удаление из чата, например,
                   //или назначение админом,
                   //добавление меня в группу
        //и проч (изменение ника пользователя, аватарки)
    }
    //AdminAction
    public sealed record AdminActionRequestSettings(
        AdminActionRequestSettingsMethod method,
        string body);
    public enum AdminActionRequestSettingsMethod
    {
        AddParticipant, //body - username
        RemoveParticipant, //body - username
        Empowerment,//сделать админом //body - username
                     //можно список прав еще:
                     //возможность удалять сообщения, назначать админом других
        Abolition//убрать права админа //body - username
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
    //response:
    //User - отдельная модель (byte[] авы, имя и проч (статус, которого пока нет))
    //chat - отдельная модель (byte[] авы, имя, список участников (для группы), короче сложная модель)

    //ChangeSettings
    public sealed record ChangeSettingsRequestSettings(
        string username,
        string name//и проч (аватарка, дата рождения, статус)
        );
    //response - string.empty
}
