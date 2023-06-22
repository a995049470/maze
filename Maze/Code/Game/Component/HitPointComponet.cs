using Stride.Core;
using Stride.Engine;

namespace Maze.Code.Game
{
    /// <summary>
    /// 血量组件
    /// </summary>
    [DataContract]
    public class HitPointComponet : StartupScript
    {
        [DataMemberIgnore]
        public int CurrentHp;
        [DataMember(10)]
        public int MaxHp = 1;

        public override void Start()
        {
            base.Start();
            CurrentHp = MaxHp;
        }

    }

}
