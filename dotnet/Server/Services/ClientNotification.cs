using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BepInEx.ModManager.Server.Services
{
    public static class ClientNotification
    {
        // Single writer should be enough for this use case
        public static ChannelWriter<LongConnectResponse> ChannelWriter { get; set; }
    }
}
