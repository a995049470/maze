using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Games;
using System;
using Stride.Core.Threading;
using Stride.Core.Collections;

namespace Maze.Code.Game
{
    public class OneShotData 
    {

    }

    public class OneShotProcessor : GameEntityProcessor<OneShotComponent, OneShotData>
    {
        private int maxDieFrame = -1;
        private FastCollection<OneShotComponent> waitDelList = new FastCollection<OneShotComponent>();

        public override void Update(GameTime time)
        {
            base.Update(time);
            var currentFrame = time.FrameCount;
            if(maxDieFrame >= currentFrame)
            {
                foreach (var oneShot in ComponentDatas.Keys)
                {
                    if(oneShot.DieFrame == currentFrame)
                    {
                        waitDelList.Add(oneShot);
                    }
                }         
                foreach (var oneShot in waitDelList)
                {
                    oneShot.Entity.Remove(oneShot);
                }     
            }
        }

        protected override OneShotData GenerateComponentData([NotNull] Entity entity, [NotNull] OneShotComponent component)
        {
            return new OneShotData();
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] OneShotComponent component, [NotNull] OneShotData associatedData)
        {
            return true;
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] OneShotComponent component, [NotNull] OneShotData data)
        {
            base.OnEntityComponentAdding(entity, component, data);
            maxDieFrame = Math.Max(maxDieFrame, component.DieFrame);
        }
    }
}
