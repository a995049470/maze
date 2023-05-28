
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;
using System;

namespace Maze.Code.Game
{
    public class AutoMoveData
    {
        public AutoMoveControllerComponent Controller;
        public VelocityComponent Velocity;
    }

    public class AutoMoveProcessor : GameEntityProcessor<AutoMoveControllerComponent, AutoMoveData>
    {
        private Random random;

        public AutoMoveProcessor() : base(typeof(VelocityComponent))
        {
            random = new Random((DateTime.Now.Millisecond + 1) * (DateTime.Now.Second + 1));
        }




        public override void Update(GameTime time)
        {
            base.Update(time);
            var simulation = GetSimulation();

            Vector3[] directions = new Vector3[]
            {
                 Vector3.UnitX,
                -Vector3.UnitX,
                 Vector3.UnitZ,
                -Vector3.UnitZ
            };
            var weights = new int[] { 1, 50, 100 };

            Dispatcher.ForEach(ComponentDatas, kvp =>
            {
                var velocity = kvp.Value.Velocity;
                var controller = kvp.Value.Controller;
                bool isIdle = velocity.TargetPos == velocity.LastTargetPos;
                if (isIdle)
                {
                    float minDis = float.MaxValue;
                    bool isAllPointOutRect = true;
                    int totalWeight = 0;
                    var center = new Vector3(controller.MoveRect.Center.X, 0, controller.MoveRect.Center.Y);
                    //xyz:pos, w:weight
                    FastList<(Vector3, int)> possbilePoints = new FastList<(Vector3, int)>();
                    for (int i = 0; i < directions.Length; i++)
                    {
                        var dir = directions[i];
                        var point = velocity.TargetPos + dir;
                        var isWay = true;
                        var isOutOfRect = !controller.MoveRect.Contains(point.X, point.Z);
                        if (simulation != null)
                        {
                            //无障碍则判定是可行走的目标
                            var hit = simulation.Raycast(velocity.TargetPos, point);
                            isWay = !hit.Succeeded;
                        }
                        if (isWay)
                        {
                            isAllPointOutRect &= isOutOfRect;
                            //根据面向方向决定下次行动的方法.
                            int d = (int)MathF.Round(Vector3.Dot(velocity.FaceDirection, dir)) + 1;
                            var weight = isOutOfRect ? 0 : weights[d];
                            totalWeight += weight;
                            var item = (point, weight);
                            if (isAllPointOutRect)
                            {
                                var dis = Vector3.DistanceSquared(center, point);
                                if (dis < minDis)
                                {
                                    minDis = dis;
                                    possbilePoints.Insert(0, item);
                                }
                                else possbilePoints.Add(item);
                            }
                            else possbilePoints.Add(item);
                        }
                    }

                    isAllPointOutRect &= possbilePoints.Count > 0;

                    int taretId = -1;
                    if (isAllPointOutRect)
                    {
                        taretId = 0;
                    }
                    else if(totalWeight > 0)
                    {
                        var r = random.Next(0, totalWeight);
                        taretId = 0;
                        for (taretId = 0; taretId < possbilePoints.Count; taretId++)
                        {
                            r -= possbilePoints[taretId].Item2;
                            if (r < 0) break;
                        }
                    }
                    if(taretId >= 0)
                    {
                        velocity.TargetPos = possbilePoints[taretId].Item1;
                        velocity.FaceDirection = velocity.TargetPos - velocity.LastTargetPos;
                    }

                }
            });
        }



        protected override AutoMoveData GenerateComponentData([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component)
        {
            var data = new AutoMoveData()
            {
                Controller = component,
                Velocity = entity.Get<VelocityComponent>()
            };
            return data;
        }



        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] AutoMoveControllerComponent component, [NotNull] AutoMoveData associatedData)
        {
            return associatedData.Controller == component && associatedData.Velocity == entity.Get<VelocityComponent>();
        }

    }


}
