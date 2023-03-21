using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Code.Map
{
    
    public class StateData<T> where T : EntityComponent
    {
        public StateMachineComponent StateMachine;
        public T Componet;
        public StateData()
        {

        }
    }

    public class StateMachineProcessor<T> : GameEntityProcessor<StateMachineComponent, StateData<T>> where T : EntityComponent
    {
        public StateMachineProcessor() : base(typeof(T))
        {

        }

        protected override StateData<T> GenerateComponentData([NotNull] Entity entity, [NotNull] StateMachineComponent component)
        {
            var data = new StateData<T>();
            data.StateMachine = component;
            data.Componet = entity.Get<T>();
            return data;
        }

        
    }
}
