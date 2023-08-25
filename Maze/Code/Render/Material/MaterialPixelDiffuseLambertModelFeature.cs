using Stride.Core;
using Stride.Rendering.Materials;
using Stride.Rendering;
using Stride.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Render
{
    [DataContract("MaterialPixelDiffuseLambertModelFeature")]
    [Display("PixelLambert")]
    public class MaterialPixelDiffuseLambertModelFeature : MaterialFeature, IMaterialDiffuseModelFeature, IEnergyConservativeDiffuseModelFeature
    {
        [DataMemberIgnore]
        bool IEnergyConservativeDiffuseModelFeature.IsEnergyConservative { get; set; }

        private bool IsEnergyConservative => ((IEnergyConservativeDiffuseModelFeature)this).IsEnergyConservative;

        public override void GenerateShader(MaterialGeneratorContext context)
        {
            var shaderBuilder = context.AddShading(this);
            shaderBuilder.LightDependentSurface = new ShaderClassSource("PixelMaterialSurfaceShadingDiffuseLambert", IsEnergyConservative);
        }

        public bool Equals(MaterialPixelDiffuseLambertModelFeature other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEnergyConservative.Equals(other.IsEnergyConservative);
        }

        public bool Equals(IMaterialShadingModelFeature other)
        {
            return Equals(other as MaterialPixelDiffuseLambertModelFeature);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as MaterialPixelDiffuseLambertModelFeature);
        }

        public override int GetHashCode()
        {
            return IsEnergyConservative.GetHashCode();
        }
        public virtual string GetShadingModelName()
        {
            return "MaterialSurfacePixelShading";
        }
    }
}
