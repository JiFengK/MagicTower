﻿using System;

namespace MagicTower.Data
{
    [Serializable]
    public class TileMapData
    {
        public uint Level;
        public uint Width;
        public uint Height;
        public int[,] FloorLayer;
        public PortalData[] PortalDatas;
        public MonsterData[] MonsterDatas;
        public ItemData[] ItemDatas;
    }
}
