using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using NLog;

namespace BepInEx.ModManager.Server.Services
{
    public static class ClientNotification
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        // Single writer should be enough for this use case
        public static ChannelWriter<LongConnectResponse> ChannelWriter { get; set; }

        public static async Task WriteAsync(LongConnectResponse message)
        {
            if (ChannelWriter != null)
            {
                try
                {
                    await ChannelWriter.WriteAsync(message).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
