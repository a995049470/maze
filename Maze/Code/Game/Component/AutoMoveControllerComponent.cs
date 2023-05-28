
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Games;
using System;

namespace Maze.Code.Game
{

    [DefaultEntityComponentProcessor(typeof(AutoMoveProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class AutoMoveControllerComponent : EntityComponent
    {
        [DataMember(10)]
        public RectangleF MoveRect;
    }

    



}
