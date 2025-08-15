using System;
using System.Collections.Generic;
using Core.Signals;
using Game.Enums;
using Game.UI.Panel;
using UnityEngine;
using Zenject;

namespace Game.UI
{
    public class UIController : MonoBehaviour
    {
        private SignalBus signalBus;
        private GameController gameController;
        private IPanelService panelService;
        
        private GameFinishPanelBase currentPanel;
        
        private readonly Dictionary<LevelState, Type> _panelMap = new()
        {
            { LevelState.Playing, typeof(InGamePanel) },
            { LevelState.GameOver, typeof(LosePanel) },
            { LevelState.Completed, typeof(WinPanel) }
        };
        
        [Inject]
        private void Construct(SignalBus signalBus, GameController gameController, PanelService panelService)
        {
            this.signalBus = signalBus;
            this.panelService = panelService;
            this.gameController = gameController;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<LevelStateChangedSignal>(OnLevelStateChanged);
        }
        
        private void OnDisable()
        {
            signalBus.TryUnsubscribe<LevelStateChangedSignal>(OnLevelStateChanged);
        }
        
        private async void OnLevelStateChanged(LevelStateChangedSignal args)
        {
            if (!_panelMap.TryGetValue(args.State, out var panelType))
                return;

            var panel = await panelService.ShowPanel(panelType);
            if (panel is GameFinishPanelBase pb)
            {
                currentPanel = pb;
                currentPanel.OnButtonPressed += OnButtonPressed;
            }
        }
        
        private void OnButtonPressed()
        {
            if (currentPanel != null)
            {
                currentPanel.OnButtonPressed -= OnButtonPressed;
            }
            gameController.StartGame();
        }
    }
}