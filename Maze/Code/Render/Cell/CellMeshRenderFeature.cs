using Stride.Rendering;
using System;

namespace Maze.Code.Render
{
    public class CellMeshRenderFeature : MeshRenderFeature
    {
        public override Type SupportedRenderObjectType => typeof(CellRenderMesh);
    }
}
