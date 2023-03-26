using Stride.Animations;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Rendering;

namespace Maze.Code.Map
{
    public class PoseData
    {
        public AnimationUpdater AnimationUpdater;
        public PoseComponent PoseComponent;
        public AnimationClipResult AnimationClipResult;
    }
    public class PoseProcessor : GameEntityProcessor<PoseComponent, PoseData>
    {
        private readonly ConcurrentPool<FastList<AnimationOperation>> animationOperationPool = new ConcurrentPool<FastList<AnimationOperation>>(() => new FastList<AnimationOperation>());
        protected override PoseData GenerateComponentData([NotNull] Entity entity, [NotNull] PoseComponent component)
        {
            return new PoseData
            {
                PoseComponent = component,
            };
        }

        protected override bool IsAssociatedDataValid([NotNull] Entity entity, [NotNull] PoseComponent component, [NotNull] PoseData associatedData)
        {
            return component == associatedData.PoseComponent;
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] PoseComponent component, [NotNull] PoseData data)
        {
            data.AnimationUpdater = new AnimationUpdater();
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] PoseComponent component, [NotNull] PoseData data)
        {
            if(data.PoseComponent.PlayingAnimation != null)
            {
                var evaluator = data.PoseComponent.PlayingAnimation.Evaluator;
                if (evaluator != null)
                {
                    data.PoseComponent.Blender.ReleaseEvaluator(evaluator);
                    data.PoseComponent.PlayingAnimation.Evaluator = null;
                }
            }
            if (data.AnimationClipResult != null)
                data.PoseComponent.Blender.FreeIntermediateResult(data.AnimationClipResult);

        }

        public override void Draw(RenderContext context)
        {
            Dispatcher.ForEach(ComponentDatas, () => animationOperationPool.Acquire(), (entity, animationOperations) =>
            {
                var associatedData = entity.Value;

                var animationUpdater = associatedData.AnimationUpdater;
                var poseComponent = associatedData.PoseComponent;
                {
                    var playingAnimation = poseComponent.PlayingAnimation;
                    if (playingAnimation != null)
                    {
                        var evaluator = playingAnimation.Evaluator;
                        // Create evaluator
                        if (evaluator == null)
                        {
                            evaluator = poseComponent.Blender.CreateEvaluator(playingAnimation.Clip);
                            playingAnimation.Evaluator = evaluator;
                        }
                        animationOperations.Add(CreatePushOperation(playingAnimation));
                    }
                }

                if (animationOperations.Count > 0)
                {
                    // Animation blending
                    poseComponent.Blender.Compute(animationOperations, ref associatedData.AnimationClipResult);
                    // Update animation data if we have a model component
                    animationUpdater.Update(poseComponent.Entity, associatedData.AnimationClipResult);
                }      
                animationOperations.Clear();
            }, animationOperations => animationOperationPool.Release(animationOperations));
        }

        private AnimationOperation CreatePushOperation(PlayingAnimation playingAnimation)
        {
            return AnimationOperation.NewPush(playingAnimation.Evaluator, playingAnimation.CurrentTime);
        }
    }
}
