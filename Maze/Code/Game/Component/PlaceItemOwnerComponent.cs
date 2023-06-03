using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;

namespace Maze.Code.Game
{
   
    //刚放置过炸药的情况
    [DataContract]
    public class PlaceItemOwnerComponent : EntityComponent
    {
        [DataMemberIgnore]
        public Guid OwnerId;
        
    }

}
