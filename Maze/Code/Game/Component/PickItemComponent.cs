using Stride.Core;
using System;

namespace Maze.Code.Game
{
    //标记捡到的物体id
    [DataContract]
    public class PickedItemComponent : OneShotComponent
    {
        public Guid ItemId;

        public PickedItemComponent(int bornFrame, int life = -1) : base(bornFrame, life)
        {

        }
    }

}
