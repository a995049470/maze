﻿using Stride.Core.Annotations;
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
        public RenderCellSprite CellSprite;
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
                var cellSprite = spriteStateKeyPair.Value.CellSprite;
                var currentSprite = component.CurrentSprite;

                cellSprite.Enabled = component.Enabled;

                if (cellSprite.Enabled)
                {
                    cellSprite.WorldMatrix = component.Entity.Transform.WorldMatrix;
                    cellSprite.RotationEulerZ = component.Entity.Transform.RotationEulerXYZ.Z;

                    cellSprite.RenderGroup = component.RenderGroup;

                    cellSprite.Sprite = currentSprite;
                    cellSprite.SpriteType = component.SpriteType;
                    cellSprite.IgnoreDepth = component.IgnoreDepth;
                    cellSprite.Sampler = component.Sampler;
                    cellSprite.BlendMode = component.BlendMode;
                    cellSprite.Swizzle = component.Swizzle;
                    cellSprite.IsAlphaCutoff = component.IsAlphaCutoff;
                    cellSprite.PremultipliedAlpha = component.PremultipliedAlpha;
                    // Use intensity for RGB part
                    cellSprite.Color = component.Color * component.Intensity;
                    cellSprite.Color.A = component.Color.A;
                    cellSprite.CellValue = component.CellValue;

                    cellSprite.CalculateBoundingBox();
                }

                // TODO Should we allow adding RenderSprite without a CurrentSprite instead? (if yes, need some improvement in RenderSystem)
                var isActive = (currentSprite != null) && cellSprite.Enabled;
                if (spriteStateKeyPair.Value.Active != isActive)
                {
                    spriteStateKeyPair.Value.Active = isActive;
                    if (isActive)
                        VisibilityGroup.RenderObjects.Add(cellSprite);
                    else
                        VisibilityGroup.RenderObjects.Remove(cellSprite);
                }
            }
        }


        protected override CellSpriteInfo GenerateComponentData([NotNull] Entity entity, [NotNull] CellSpriteComponent component)
        {
            return new CellSpriteInfo { CellSprite = new RenderCellSprite { Source = component } };
        }

        protected override void OnEntityComponentRemoved(Entity entity, CellSpriteComponent component, CellSpriteInfo data)
        {
            VisibilityGroup.RenderObjects.Remove(data.CellSprite);
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] CellSpriteComponent component, [NotNull] CellSpriteInfo associatedData)
        {
            return associatedData.CellSprite.Source == component;
        }
    }

}
