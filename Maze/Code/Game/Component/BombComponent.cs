using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace Maze.Code.Game
{
    public enum BombState
    {
        Sleep,
        ReadyBoom,
        AfterBoom
    }

    [DefaultEntityComponentProcessor(typeof(BombProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [DataContract]
    public class BombComponent : EntityComponent
    {
        
        public int AttackRange = 1;
        private int setUpTime = 1;
        public int SetUpTime
        {
            get => setUpTime;
            set
            {
                setUpTime = value;
                SetUpTimer = new TimerF(0, setUpTime);
            }
        }
        [DataMemberIgnore]
        public TimerF SetUpTimer;
        [DataMemberIgnore]
        public BombState State = BombState.Sleep;
        
    }

}
