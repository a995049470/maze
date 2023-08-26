using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Core.Serialization.Contents;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Physics;
using Stride.Rendering;
using System.Threading;

namespace Maze.Code.Game
{

    public class LevelManager : IManager
    {
        private IServiceRegistry service;
        public LevelManager()
        {
            
        }

        public void Initialize(IServiceRegistry _service)
        {
            service = _service;
        }
        
        public void LoadLevel(string url, Int2 origin)
        {
            var loader = new LevelLoader(service, origin, url);
            loader.LoadLevel();
        }



    }
}
