using Ruffles.Channeling;

namespace ACSSandbox.Common.Network.Ruffles
{
    public static class NetworkSetup
    {
        public static ChannelType[] ChannelTypes =>
            new[] { ChannelType.Reliable, ChannelType.Unreliable, ChannelType.ReliableOrdered };
    }
}
