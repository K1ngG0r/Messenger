using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Connection
{
    public sealed record UpdateResponse(
        UpdateResponseMethod method,
        string payload);
    public enum UpdateResponseMethod
    {
        NewMessage,
        NewChat
    }
}
