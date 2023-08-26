using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI.Panels;
using Stride.UI;

namespace Maze.Code.Game
{
    public class UIManager : IManager
    {
        private const string uiRootPrefabUrl = "Prefab/UI/UIRoot";
        private Panel root;
        protected IServiceRegistry service;
        protected ContentManager content;
        protected SceneSystem sceneSystem;

        public UIManager()
        {
            
        }

        public void Initialize(IServiceRegistry _service)
        {
            service = _service;
            content = _service.GetSafeServiceAs<ContentManager>();
            sceneSystem = _service.GetSafeServiceAs<SceneSystem>();

            //加载初始UI
            var prefab = content.Load<Prefab>(uiRootPrefabUrl);
            var entity = prefab.Instantiate()[0];
            root = entity.Get<UIComponent>().Page.RootElement as Panel;
            sceneSystem.SceneInstance.RootScene.Entities.Add(entity);
            root.Children.Add(CreateWindow<UIWindow_Main>());
        }

        private UIElement CreateWindow<T>() where T : UIWindow, new()
        {
            var window = new T();
            window.OnWindowCreate(service);
            return window.Root;
        }
    }
}
