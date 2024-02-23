using System;

namespace ACSSandbox.Common.Network
{
    public struct NetworkId : IComparable<NetworkId>, IEquatable<NetworkId>
    {
        public static NetworkId None = new NetworkId();
        private Guid id;

        public static NetworkId Create()
        {
            return new NetworkId { id = Guid.NewGuid() };
        }

        public bool Equals(NetworkId other)
        {
            return id.Equals(other.id);
        }

        public override bool Equals(object other)
        {
            if (other is NetworkId)
            {
                return Equals((NetworkId)other);
            }

            return false;
        }

        public static bool operator ==(NetworkId one, NetworkId other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(NetworkId one, NetworkId other)
        {
            return !one.Equals(other);
        }

        public int CompareTo(NetworkId other)
        {
            return id.CompareTo(other.id);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return id.ToString("N");
        }
    }
}
