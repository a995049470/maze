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
using System.Collections.Concurrent;

namespace Maze.Code.Game
{
    public abstract class GameEntityProcessor<TComponent, TData> : EntityProcessor<TComponent, TData> where TData : class where TComponent : EntityComponent
    {
       
        protected IGame game;
        protected InputManager input;
        protected SceneSystem sceneSystem;
        protected ContentManager content;
        protected Logger log;
        protected ConcurrentBag<Entity> cacheEntities = new ConcurrentBag<Entity>();
        protected ConcurrentBag<Action> cacheActions = new ConcurrentBag<Action>();



        protected static Vector3 defaultScale = Vector3.One;
        protected static Quaternion defaultRotation = Quaternion.Identity;
        protected const float tiny = 0.001f;

        protected GameEntityProcessor([NotNull] params Type[] requiredAdditionalTypes)
            : base(requiredAdditionalTypes)
        {
        }

        
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

        /// <summary>
        /// 物体坐标转最近的格子坐标
        /// </summary>
        /// <returns></returns>
        protected Vector3 PosToGridCenter(Vector3 pos)
        {
            pos.X = MathF.Round(pos.X);
            pos.Y = 0;
            pos.Z = MathF.Round(pos.Z);
            return pos;
        }

        protected void InvokeCacheActions()
        {
            Action action;
            while (!cacheActions.IsEmpty)
            {
                if(cacheActions.TryTake(out action))
                {
                    action.Invoke();
                }

            }
        }

        protected T GetProcessor<T>() where T : EntityProcessor
        {
            return sceneSystem.SceneInstance.GetProcessor<T>();
        }

      
    }
}
