using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;
using Stride.Physics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Maze.Code.Game
{
    public class BombData
    {
        public BombComponent Bomb;
        public TransformComponent Transform;
        public PlaceItemOwnerComponent Owner;
    }

    public class BombProcessor : GameEntityProcessor<BombComponent, BombData>
    {
        private readonly static Vector3 farPos = Vector3.One * 65536;
        public Dictionary<Guid, int> OwnerBombCountDic = new Dictionary<Guid, int>();
        
        private ThreadSafePool<FastCollection<HitResult>> hitResutlListPool = new ThreadSafePool<FastCollection<HitResult>>();
        private PlacerProcessor placerProcessor;
        private Simulation simulation;
         
        public BombProcessor() : base(typeof(TransformComponent), typeof(PlaceItemOwnerComponent))
        {      
            Order = ProcessorOrder.Attack;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            float delatTime = (float)time.Elapsed.TotalSeconds;
            simulation = GetSimulation();
            placerProcessor = placerProcessor ?? GetProcessor<PlacerProcessor>();
            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var data = kvp.Value;
                //放置者走出范围时激活
                if(data.Bomb.State == BombState.Sleep)
                {
                    var bombPos = data.Transform.Position;
                    var ownerGridPos = GetOwnerGridPosition(data);
                    bool isLeaveBomb = Vector3.DistanceSquared(ownerGridPos, bombPos) > tiny;               
                    if (isLeaveBomb)
                    {
                        data.Transform.Scale = Vector3.One * 1.0f;
                        data.Bomb.State = BombState.ReadyBoom;
                    }
                }
                //准备爆炸
                else if(data.Bomb.State == BombState.ReadyBoom)
                {
                    if(data.Bomb.SetUpTimer.Run(delatTime))
                    {
                        data.Bomb.State = BombState.AfterBoom;
                        //爆炸造成伤害

                        var attackRange = data.Bomb.AttackRange;
                        var pos = data.Transform.Position;
                        
                        var list = hitResutlListPool.Take();
                        //纵向检测
                        {
                            var from = pos + (attackRange + 1.0f) * Vector3.UnitX;
                            var to = pos - (attackRange + 0.5f) * Vector3.UnitX;
                            simulation.RaycastPenetrating(from, to, list);                           
                        }
                        {
                            var from = pos + (attackRange + 1.0f) * Vector3.UnitZ;
                            var to = pos - (attackRange + 0.5f) * Vector3.UnitZ;
                            simulation.RaycastPenetrating(from, to, list);
                        }
                        //可能重复给予伤害的问题
                        foreach (var hitResult in list)
                        {
                            if (!hitResult.Succeeded) continue;
                            var entity = hitResult.Collider.Entity;
                            //TODO:考虑使用索引去获取...
                            var hurt = entity.Get<HurtComponet>();
                            if (hurt == null) continue;
                            Interlocked.Add(ref hurt.HurtValue, 1);
                        }
                        //爆炸表现
                        //直接销毁
                        cacheActions.Add(() =>
                        {
                            RemoveEntity(data.Bomb.Entity);
                        });
                    }
                }
            });
            InvokeCacheActions();
        }

        private Vector3 GetOwnerGridPosition(BombData data)
        {
            TransformComponent ownerTransform = null;
            placerProcessor?.PlacerTransformDic.TryGetValue(data.Owner.OwnerId, out ownerTransform);
            
            var ownerGridPos = ownerTransform == null ? farPos : PosToGridCenter(ownerTransform.Position);
            return ownerGridPos;
        }

        protected override BombData GenerateComponentData([NotNull] Entity entity, [NotNull] BombComponent component)
        {
            return new BombData()
            {
                Bomb = component,            
                Transform = entity.Get<TransformComponent>(),
                Owner = entity.Get<PlaceItemOwnerComponent>()
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] BombComponent component, [NotNull] BombData associatedData)
        {
            return associatedData.Bomb == component &&
                associatedData.Transform == entity.Get<TransformComponent>() &&
                associatedData.Owner == entity.Get<PlaceItemOwnerComponent>();
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] BombComponent component, [NotNull] BombData data)
        {
            base.OnEntityComponentAdding(entity, component, data);
            if (!OwnerBombCountDic.TryGetValue(data.Owner.OwnerId, out var num)) num = 0;
            num++;
            OwnerBombCountDic[data.Owner.OwnerId] = num;
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] BombComponent component, [NotNull] BombData data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
            OwnerBombCountDic[data.Owner.OwnerId]--;
        }

        public int GetOwnerBombCount(Guid id)
        {
            if(!OwnerBombCountDic.TryGetValue(id, out var bombCount)) bombCount = 0;
            return bombCount;

        }
    }
}
