using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.UI;

namespace Maze.Code.Game
{

    public abstract class UIWindow
    {
        protected abstract string Url { get; }
        protected abstract UIWindowFlag Flag { get; }
        protected IServiceRegistry service;
        protected ContentManager content;
        protected SceneSystem sceneSystem;
        protected UIElement root;
        protected Logger log;
        
        public UIElement Root { get => root; }
        

        public UIWindow()
        {
           
        }
  

        private void Load()
        {
            var uiPage = content.Load<UIPage>(Url);
            root = uiPage.RootElement;
        }

        protected virtual void InitServices(IServiceRegistry _service)
        {
            service = _service;
            content = _service.GetSafeServiceAs<ContentManager>();
            sceneSystem = _service.GetSafeServiceAs<SceneSystem>();
            var className = GetType().FullName;
            log = GlobalLogger.GetLogger(className);
        }   

        public void OnWindowCreate(IServiceRegistry _service)
        {
            InitServices(_service);
            Load();
            BindUIElements();
        }
        
        protected virtual void BindUIElements() { }
    }
}
