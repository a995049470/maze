using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Processors;
using Stride.Graphics;
using Stride.Rendering;
using Quaternion = Stride.Core.Mathematics.Quaternion;

namespace Maze.Code.Render
{
    public class CellRenderView : RenderView
    {
        public Texture CellTexture;

        private static CameraComponent camera;
        public static CameraComponent Camera
        {
            get
            {
                if(camera == null)
                {
                    var entity = new Entity();
                    camera = new CameraComponent(0, 100);
                    entity.Add(camera);
                    entity.Transform.Position = new Vector3(0, 50, 0);
                    entity.Transform.Rotation = Quaternion.LookRotation(-Vector3.UnitY, Vector3.UnitX);
                    entity.Transform.UpdateLocalMatrix();
                    camera.UseCustomAspectRatio = true;
                    camera.Projection = CameraProjectionMode.Orthographic;              
                }
                return camera;
            }
        }

        public static void UpdateRenderRectToRenderView(RectangleF drawRect, RenderView renderView)
        {
            var pos = Camera.Entity.Transform.Position;
            pos.X = drawRect.Center.X;
            pos.Z = drawRect.Center.Y;
            Camera.Entity.Transform.Position = pos;
            Camera.Entity.Transform.UpdateWorldMatrix();

            Camera.AspectRatio = drawRect.Height / drawRect.Width;
            Camera.OrthographicSize = drawRect.Height;
            Camera.Update(null);

            renderView.View = Camera.ViewMatrix;
            renderView.Projection = Camera.ProjectionMatrix;
            renderView.NearClipPlane = Camera.NearClipPlane;
            renderView.FarClipPlane = Camera.FarClipPlane;
            renderView.Frustum = Camera.Frustum;

            // Enable frustum culling
            renderView.CullingMode = CameraCullingMode.Frustum;

            Matrix.Multiply(ref renderView.View, ref renderView.Projection, out renderView.ViewProjection);
        }
        
           
    }

}