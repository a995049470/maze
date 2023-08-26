using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI.Panels;
using Stride.UI;
using Stride.Core.Collections;
using Stride.Core.Threading;

namespace Maze.Code.Game
{
    public class UIManager : SyncScript
    {
        private const string uiRootPrefabUrl = "Prefab/UI/UIRoot";
        private Panel root;
        protected IServiceRegistry service;
        protected ContentManager content;
        protected SceneSystem sceneSystem;
        protected FastCollection<UIWindow> uiWindows;

        public UIManager()
        {
            uiWindows = new FastCollection<UIWindow>();
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
            OpenWindw<UIWindow_Main>(UIWindowFlag.Main);
        }

        public override void Update()
        {
            foreach(var uiWindow in uiWindows)
            {
                if(uiWindow != null && uiWindow.IsDitry)
                {
                    uiWindow.Refresh();
                }
            };
        }

        /// <summary>
        /// 生成一个T类型的UIWindow当作目标Flag的窗口(UIWindow一定会生成在root下)
        /// </summary>
        /// <param name="flag"></param>
        /// <typeparam name="T"></typeparam>
        public void OpenWindw<T>(UIWindowFlag flag) where T : UIWindow, new()
        {
            bool isGet = TryGetUIWindow(flag, out var window);
            if(isGet && window is not T)
            {
                root.Children.Remove(window.Root);
                window.Close(true);
                isGet = false;
            }
            if(!isGet)
            {
                window = CreateUIModule<T>();
                uiWindows[((int)flag)] = window;
                root.Children.Add(window.Root);
            }
            window.Open();
        }

        public void CloseWindow(UIWindowFlag flag, bool isDestory = false)
        {
            bool isGet = TryGetUIWindow(flag, out var window);
            if(isDestory)
            {
                uiWindows[((int)flag)] = null;
                root.Children.Remove(window.Root);
            }
            if(isGet) window.Close(isDestory);
        }


        private bool TryGetUIWindow(UIWindowFlag flag, out UIWindow window) 
        {
            var id = ((int)flag);
            window = null;
            if(id < uiWindows.Count)
            {
                window = uiWindows[id];
            }
            else
            {
                var num = id - uiWindows.Count + 1;
                uiWindows.AddRange(new UIWindow[num]);
            }
            return window != null;
        }

        //创建UI组件
        public T CreateUIModule<T>() where T : UIModle, new()
        {
            var module = new T();
            module.OnCreate(service);
            return module;
        }

       
    }
}
