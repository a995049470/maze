using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{

    /// <summary>
    /// 受伤组件(负数算回复, 后续分离)
    /// </summary>
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(HurtProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class HurtComponet : EntityComponent
    {
        [DataMemberIgnore]
        public int HurtValue;
    }

}
