using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Map
{
    public class StaticData : ISpriteAsset
    {
        public string AssetUrl;
        public int FrameIndex;
        public int Layer;

        public string GetAssetPath()
        {
            return AssetUrl;
        }

        public int GetFrameIndex()
        {
            return FrameIndex;
        }
    }
}
