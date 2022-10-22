using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Map
{
    public interface ISpriteAsset
    {
        string GetAssetPath();
        int GetFrameIndex();
    }

}
