namespace DataTypes
{
    public class Config
    {
        public int sendPacketsCount = 3; // anything above 3 results in TooManyPackets
        public int PacketSpreadMs { get; set; }
        public Config()
        { 
            PacketSpreadMs = 31;
        }
    }
}
