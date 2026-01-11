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
    //response - string.empty

    //Send
    public sealed record SendRequestSettings(
        Guid chatId,
        string message);
    //response - string.empty

    //Update
    //request - string.empty
    public sealed record UpdateResponseSettings(
        List<SingleChange> changes);
    public sealed record SingleChange(
        SingleChangeMethod method,
        string body);
    public enum SingleChangeMethod
    {
        NewMessage,
        NewChat,
        AdminAction
    }
    //AdminAction
    public sealed record AdminActionRequestSettings(
        AdminActionRequestSettingsMethod method,
        string body);
    public enum AdminActionRequestSettingsMethod
    {
        AddParticipant,
        RemoveParticipant
        //и тд
    }
    //response - string.empty

    //CreateChat
    public sealed record CreateChatRequestSettings(
        CreateChatRequestSettingsMethod method,
        string body);
    public enum CreateChatRequestSettingsMethod
    {
        PrivateChat,
        GroupChat,
        ChannelChat
    }
    //response - guid чата

    //LoadAvatar
    public sealed record LoadAvatarRequestSettings(
        LoadAvatarRequestSettingsMethod method,
        string body);
    public enum LoadAvatarRequestSettingsMethod
    {
        User,
        Chat
    }
    //response - byte[] аватарки
}
