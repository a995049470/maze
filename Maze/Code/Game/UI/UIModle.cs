using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.UI;

namespace Maze.Code.Game
{
    public abstract class UIModle
    {
        //表示UIModel是否需要刷新
        public bool IsDitry { get; private set; }
        protected abstract string Url { get; }
        protected IServiceRegistry service;
        protected ContentManager content;
        protected SceneSystem sceneSystem;
        protected UIElement root;
        protected Logger log;
        
        public UIElement Root { get => root; }
        

        public UIModle()
        {
           
        }

        public void SetDitry()
        {
            IsDitry = true;
        }

        public virtual void Refresh()
        {
            IsDitry = false;
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

        public void OnCreate(IServiceRegistry _service)
        {
            InitServices(_service);
            Load();
            BindUIElements();
        }
        
        protected virtual void BindUIElements() { }
    }
}
