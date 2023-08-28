using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    [DefaultEntityComponentProcessor(typeof(OneShotProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class OneShotComponent : EntityComponent
    {
        //只有等于DieFrame才能销毁 -1意味着无法销毁
        public OneShotComponent(int bornFrame, int life = -1)
        {
            BornFrame = bornFrame;
            DieFrame = bornFrame + life;
        }
        [DataMemberIgnore]
        public int BornFrame;
        [DataMemberIgnore]
        public int DieFrame;
    }

}
