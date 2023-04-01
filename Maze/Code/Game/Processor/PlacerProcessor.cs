
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Physics;
using Stride.Rendering.Sprites;
using System;

namespace Maze.Code.Game
{
    public class PlacerData
    {
        public PlacerComponent Placer;
        public TransformComponent Transform;
    }

    public class PlacerProcessor : GameEntityProcessor<PlacerComponent, PlacerData>
    {
        private Simulation simulation;
        public PlacerProcessor() : base(typeof(TransformComponent))
        {

        }

        private void PlaceItem(PlacerData data)
        {                 
            //没有正在放置的物体
            if(data.Placer.PlaceItem == null)
            {
                //检测能否放置
                var shape = new BoxColliderShape(true, Vector3.One);
                var pos = data.Transform.Position;
                pos.X = MathF.Round(pos.X);
                pos.Y = MathF.Round(pos.Y);
                pos.Z = SpriteUtils.PlaceItemZ;
                Matrix.Transformation(ref defaultScale, ref defaultRotation, ref pos, out var from);
                var hitResult = simulation.ShapeSweep(shape, from, from);
                var isCanPlace = !hitResult.Succeeded;
                if(isCanPlace)
                {
                    //开始创建实体
                    var entity = new Entity();
                    var sprite = entity.GetOrCreate<SpriteComponent>();
                    var sheetAsset = content.Load<SpriteSheet>(data.Placer.AssetUrl);
                    sprite.Color = new Color4(1, 1, 1, 0.3f);
                    sprite.Sampler = SpriteSampler.PointClamp;
                    sprite.BlendMode = SpriteBlend.AlphaBlend;
                    sprite.PremultipliedAlpha = false;
                    if(sprite.SpriteProvider is SpriteFromSheet spriteFromSheet)
                    {
                        spriteFromSheet.Sheet = sheetAsset;
                        spriteFromSheet.CurrentFrame = data.Placer.FrameIndex;
                    }
                    entity.Name = "PlaceItem";
                    sceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                    entity.Transform.Position = pos;
                    data.Placer.PlaceItem = entity;
                }
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            simulation = simulation ?? GetSimulation();
            foreach (var data in ComponentDatas.Values)
            {
                if(data.Placer.IsReadyPlace)
                {
                    data.Placer.IsReadyPlace = false;
                    PlaceItem(data);
                }
                if(data.Placer.PlaceItem != null)
                {
                    BoundingBox box1;
                    { 
                        var pos = data.Transform.Position;
                        var min = new Vector3(pos.X - data.Placer.SafeShape.BoxSize.X * 0.5f, pos.Y - data.Placer.SafeShape.BoxSize.Y * 0.5f, 0);
                        var max = new Vector3(pos.X + data.Placer.SafeShape.BoxSize.X * 0.5f, pos.Y + data.Placer.SafeShape.BoxSize.Y * 0.5f, 0);
                        box1 = new BoundingBox(min, max);
                    }

                    BoundingBox box2;
                    {
                        var pos = data.Placer.PlaceItem.Transform.Position;
                        var min = new Vector3(pos.X - 0.5f, pos.Y - 0.5f, 0);
                        var max = new Vector3(pos.X + 0.5f, pos.Y + 0.5f, 0);
                        box2 = new BoundingBox(min, max);
                    }
                    var isIntersec = CollisionHelper.BoxIntersectsBox(ref box1, ref box2);
                    if(!isIntersec)
                    {
                        var sprite = data.Placer.PlaceItem.Get<SpriteComponent>();
                        sprite.Color = new Color(255, 255, 255, 255);
                        var staticCollider = new StaticColliderComponent();
                        var boxShapeDesc = new BoxColliderShapeDesc()
                        {
                            Is2D = true,
                            Size = Vector3.One
                        };
                        staticCollider.ColliderShapes.Add(boxShapeDesc);
                        data.Placer.PlaceItem.Add(staticCollider);
                        data.Placer.PlaceItem = null;
                        
                    }
                }
            }
        }


        protected override PlacerData GenerateComponentData([NotNull] Entity entity, [NotNull] PlacerComponent component)
        {
            return new PlacerData()
            {
                Placer = component,
                Transform = entity.Transform,
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PlacerComponent component, [NotNull] PlacerData associatedData)
        {
            return associatedData.Placer == component && associatedData.Transform == entity.Transform;
        }
    }
}
