using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Processors;
using Stride.Rendering;
using Quaternion = Stride.Core.Mathematics.Quaternion;

namespace Maze.Code.Render
{
    public class CellRenderView : RenderView
    {
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
            var pos = camera.Entity.Transform.Position;
            pos.X = drawRect.Center.X;
            pos.Z = drawRect.Center.Y;
            camera.Entity.Transform.Position = pos;
            camera.Entity.Transform.UpdateWorldMatrix();

            camera.AspectRatio = drawRect.Height / drawRect.Width;
            camera.OrthographicSize = drawRect.Height;
            camera.Update(null);

            renderView.View = camera.ViewMatrix;
            renderView.Projection = camera.ProjectionMatrix;
            renderView.NearClipPlane = camera.NearClipPlane;
            renderView.FarClipPlane = camera.FarClipPlane;
            renderView.Frustum = camera.Frustum;

            // Enable frustum culling
            renderView.CullingMode = CameraCullingMode.Frustum;

            Matrix.Multiply(ref renderView.View, ref renderView.Projection, out renderView.ViewProjection);
        }
        
           
    }

}