
using Stride.Rendering;

namespace Maze.Code.Render
{

    public class CellModelRenderProcessor : BaseModelRenderProcessor<CellModelComponent, CellRenderMesh>
    {
        protected override void UpdateMaterial(CellRenderMesh renderMesh, MaterialPass materialPass, MaterialInstance modelMaterialInstance, CellModelComponent modelComponent)
        {
            base.UpdateMaterial(renderMesh, materialPass, modelMaterialInstance, modelComponent);
            renderMesh.Flag = modelComponent.Flag;
        }
    }
}
