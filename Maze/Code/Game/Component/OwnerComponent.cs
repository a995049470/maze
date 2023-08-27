using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;

namespace Maze.Code.Game
{  
    //记录拥有者的ID
    [DataContract]
    public class OwnerComponent : OneShotEntityComponent
    {
        [DataMemberIgnore]
        public Guid OwnerId;

        public OwnerComponent(int bornFrame, int life = -1) : base(bornFrame, life)
        {
            
        }
    }

}
