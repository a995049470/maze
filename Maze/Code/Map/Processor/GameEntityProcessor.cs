using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Games;
using Stride.Input;
using Stride.Physics;
using System;

namespace Maze.Code.Map
{
    public abstract class GameEntityProcessor<TComponent, TData> : EntityProcessor<TComponent, TData> where TData : class where TComponent : EntityComponent
    {
        protected GameEntityProcessor([NotNull] params Type[] requiredAdditionalTypes)
            : base(requiredAdditionalTypes)
        {
        }
        protected IGame game;
        protected InputManager input;
        protected SceneSystem sceneSystem;
        protected ContentManager content;
        protected Logger log;
       


        protected static Vector3 defaultScale = Vector3.One;
        protected static Quaternion defaultRotation = Quaternion.Identity;
        
        protected Simulation GetSimulation()
        {
            return sceneSystem.SceneInstance.GetProcessor<PhysicsProcessor>()?.Simulation;
        }

        protected override void OnSystemAdd()
        {
            game = Services.GetSafeServiceAs<IGame>();
            input = Services.GetSafeServiceAs<InputManager>();       
            sceneSystem = Services.GetSafeServiceAs<SceneSystem>();
            content = Services.GetSafeServiceAs<ContentManager>();
            var className = GetType().FullName;
            log = GlobalLogger.GetLogger(className);
        }

      
    }
}
