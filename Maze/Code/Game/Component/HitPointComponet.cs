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
        private int currentHp;
        [DataMemberIgnore]
        public int CurrentHp { get => currentHp; set => currentHp = System.Math.Clamp(value, 0, MaxHp); }
        [DataMember(10)]
        public int MaxHp = 1;
        

        public override void Start()
        {
            base.Start();
            CurrentHp = MaxHp;
        }

        public float GetCurrentHitPointPercent()
        {
            return System.Math.Clamp((float)CurrentHp / MaxHp, 0, 1);
        }

    }

}
