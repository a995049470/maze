using Stride.Core;
using System;

namespace Maze.Code.Game
{
    //标记捡到的物体id
    [DataContract]
    public class PickItemComponent : OneShotEntityComponent
    {
        public Guid ItemId;

        public PickItemComponent(int bornFrame, int life = -1) : base(bornFrame, life)
        {
        }
    }

}
