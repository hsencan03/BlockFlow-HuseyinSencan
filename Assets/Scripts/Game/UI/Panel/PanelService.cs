using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.UI.Panel
{
    public class PanelService : IPanelService
    {
        private readonly Dictionary<Type, IPanel> panels;
        private readonly PanelHolderSo panelHolder;
        private readonly PanelFactory panelFactory;
        
        private readonly Canvas canvas;
        
        public PanelService(PanelHolderSo panelHolderSo, PanelFactory panelFactory, [Inject(Id = Constants.GameCanvasId)] Canvas gameCanvas)
        {
            panels = new Dictionary<Type, IPanel>();
            panelHolder = panelHolderSo;
            this.panelFactory = panelFactory;
            canvas = gameCanvas;
        }

        public async UniTask<T> ShowPanel<T>() where T : IPanel
        {
            await HideAllPanels();

            if (panels.TryGetValue(typeof(T), out var panel))
            {
                await panel.Show();
                return (T)panel;
            }
            
            panel = LoadPanel<T>();
            await panel.Show();
            return (T)panel;
        }
        
        public async UniTask<IPanel> ShowPanel(Type type) 
        {
            await HideAllPanels();

            if (panels.TryGetValue(type, out var panel))
            {
                await panel.Show();
                return panel;
            }
            
            panel = LoadPanel(type);
            await panel.Show();
            return panel;
        }

        public async UniTask HidePanel<T>() where T : IPanel
        {
            if (panels.TryGetValue(typeof(T), out var panel))
            {
                await panel.Hide();
            }
        }
        
        public async UniTask HideAllPanels()
        {
            foreach (var panel in panels.Values)
            {
                await panel.Hide();
            }
        }
        
        private T LoadPanel<T>() where T : IPanel
        {
            if (!panels.ContainsKey(typeof(T)))
            {
                GameObject panelPrefab = panelHolder.GetPanelPrefab<T>();
                T panel = (T)panelFactory.Create(panelPrefab);
                panel.Initialize(canvas);
                panels.Add(typeof(T), panel);

                return panel;
            }

            return default;
        }
        
        private IPanel LoadPanel(Type type) 
        {
            if (!panels.ContainsKey(type))
            {
                GameObject panelPrefab = panelHolder.GetPanelPrefab(type);
                IPanel panel = panelFactory.Create(panelPrefab);
                panel.Initialize(canvas);
                panels.Add(type, panel);

                return panel;
            }

            return default;
        }
    }
}