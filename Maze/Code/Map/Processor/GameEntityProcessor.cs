using Stride.Core;
using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Games;
using Stride.Input;
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
        protected LevelManager levelManager;


        protected override void OnSystemAdd()
        {
            game = Services.GetSafeServiceAs<IGame>();
            input = Services.GetSafeServiceAs<InputManager>();
            levelManager = Services.GetSafeServiceAs<LevelManager>();
        }
    }
}
