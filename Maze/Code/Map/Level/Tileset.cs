﻿using LitJson;
using System.Collections.Generic;
namespace Maze.Map
{
    public class Tileset
    {
        public int Uid;
        public int NumX;
        public int NumY;
        public int TileGridSize;
        public string AssetUrl;
        private Dictionary<int, JsonData> tileDataDic = new Dictionary<int, JsonData>();

        public int GetTileId(int x, int y)
        {
            return x / TileGridSize + y / TileGridSize * NumX;
        }

        public void AddJsonData(int id, JsonData data)
        {
            tileDataDic[id] = data;
        }

        public bool IsWalkable(int id)
        {
            var isWalkable = true;
            if(tileDataDic.TryGetValue(id, out var data))
            {
                if (data.ContainsKey(MapUtils.customData_isWalkable))
                {
                    isWalkable = 
                }
            }
        }
    }
}
