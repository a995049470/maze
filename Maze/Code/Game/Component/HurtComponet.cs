using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    public class OneShotEntityComponent : EntityComponent
    {
        //只有等于DieFrame才能销毁 -1意味着无法销毁
        public OneShotEntityComponent(int bornFrame, int life = -1)
        {
            BornFrame = bornFrame;
            DieFrame = bornFrame + life;
        }
        [DataMemberIgnore]
        public int BornFrame;
        [DataMemberIgnore]
        public int DieFrame;
    }

    /// <summary>
    /// 受伤组件
    /// </summary>
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(HurtProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class HurtComponet : EntityComponent
    {
        [DataMemberIgnore]
        public int HurtValue;
    }

}
