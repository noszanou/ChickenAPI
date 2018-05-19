﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChickenAPI.Packets.ServerPackets
{
    [PacketHeader("in_non_player_subpacket")]
    public class InNonPlayerSubPacketBase : PacketBase
    {
        #region Properties
        [PacketIndex(0)]
        public short Dialog { get; set; }

        [PacketIndex(1)]
        public byte Unknown { get; set; }

        #endregion
    }
}
