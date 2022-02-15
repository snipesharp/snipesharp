﻿namespace DataTypes
{
    public class Config
    {
        public int SendPacketsCount { get; set; }
        public int MillisecondsBetweenPackets { get; set; }
        public Config()
        {
            SendPacketsCount = 3;
            MillisecondsBetweenPackets = 98;
        }
    }
}
