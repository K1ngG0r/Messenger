using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Connection
{
    public sealed record UpdateResponseSettings(
        UpdateResponseSettingsMethod method,
        string payload);
    public enum UpdateResponseSettingsMethod
    {
        NewMessage,
        NewChat
    }
    public sealed record LoadAvatarRequestSettings(
        LoadAvatarRequestSettingsMethod method,
        string body);
    public enum LoadAvatarRequestSettingsMethod
    {
        User,
        Chat
    }
    public sealed record SendRequestSettings (
        Guid chatId,
        string message);
}
