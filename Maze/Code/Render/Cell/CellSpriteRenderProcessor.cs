using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Maze.Code.Render
{
    public class CellSpriteInfo
    {
        public bool Active;
        public RenderCellSprite Sprite;
    }
    public class CellSpriteRenderProcessor : EntityProcessor<CellSpriteComponent, CellSpriteInfo>, IEntityComponentRenderProcessor
    {
        public VisibilityGroup VisibilityGroup { get; set; }

        public CellSpriteRenderProcessor()
            : base(typeof(TransformComponent))
        {
        }

        public override void Draw(RenderContext gameTime)
        {
            foreach (var spriteStateKeyPair in ComponentDatas)
            {
                var component = spriteStateKeyPair.Key;
                var sprite = spriteStateKeyPair.Value.Sprite;
                var currentSprite = component.CurrentSprite;

                sprite.Enabled = component.Enabled;

                if (sprite.Enabled)
                {
                    sprite.WorldMatrix = component.Entity.Transform.WorldMatrix;
                    sprite.RotationEulerZ = component.Entity.Transform.RotationEulerXYZ.Z;

                    sprite.RenderGroup = component.RenderGroup;

                    sprite.Sprite = currentSprite;
                    sprite.SpriteType = component.SpriteType;
                    sprite.IgnoreDepth = component.IgnoreDepth;
                    sprite.Sampler = component.Sampler;
                    sprite.BlendMode = component.BlendMode;
                    sprite.Swizzle = component.Swizzle;
                    sprite.IsAlphaCutoff = component.IsAlphaCutoff;
                    sprite.PremultipliedAlpha = component.PremultipliedAlpha;
                    // Use intensity for RGB part
                    sprite.Color = component.Color * component.Intensity;
                    sprite.Color.A = component.Color.A;
                    

                    sprite.CalculateBoundingBox();
                }

                // TODO Should we allow adding RenderSprite without a CurrentSprite instead? (if yes, need some improvement in RenderSystem)
                var isActive = (currentSprite != null) && sprite.Enabled;
                if (spriteStateKeyPair.Value.Active != isActive)
                {
                    spriteStateKeyPair.Value.Active = isActive;
                    if (isActive)
                        VisibilityGroup.RenderObjects.Add(sprite);
                    else
                        VisibilityGroup.RenderObjects.Remove(sprite);
                }
            }
        }


        protected override CellSpriteInfo GenerateComponentData([NotNull] Entity entity, [NotNull] CellSpriteComponent component)
        {
            return new CellSpriteInfo { Sprite = new RenderCellSprite { Source = component } };
        }

        protected override void OnEntityComponentRemoved(Entity entity, CellSpriteComponent component, CellSpriteInfo data)
        {
            VisibilityGroup.RenderObjects.Remove(data.Sprite);
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] CellSpriteComponent component, [NotNull] CellSpriteInfo associatedData)
        {
            return associatedData.Sprite.Source == component;
        }
    }

}
