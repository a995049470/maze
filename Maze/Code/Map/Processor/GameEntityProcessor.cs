﻿using Stride.Core;
using Stride.Core.Annotations;
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
        protected LevelManager levelManager;
        protected ContentManager content;


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
            levelManager = Services.GetSafeServiceAs<LevelManager>();
            sceneSystem = Services.GetSafeServiceAs<SceneSystem>();
            content = Services.GetSafeServiceAs<ContentManager>();
        }

      
    }
}
