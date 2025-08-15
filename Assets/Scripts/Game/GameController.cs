using System;
using Core.Data;
using Core.Services.SaveService;
using Core.Signals;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.Level;
using Game.Level.LevelService;
using Game.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameController : IInitializable, IDisposable
    {
        private readonly ILevelService levelService;
        private readonly ILevelSelector levelSelector;
        private readonly ISaveService<GameSaveData> saveService;
        private readonly SignalBus signalBus;
        
        private GameConfig gameConfig;
        private GameSaveData gameSaveData;
        
        private int[] lastPlayedLevelIndex;
        private int currentPlayedArrIndex;

        private const int LastPlayedLevelArrCount = 3;
        
        [Inject]
        public GameController(ILevelService levelService, ILevelSelector levelSelector, SignalBus signalBus, ISaveService<GameSaveData> saveService, GameConfig gameConfig)
        {
            this.levelService = levelService;
            this.signalBus = signalBus;
            this.saveService = saveService;
            this.gameConfig = gameConfig;
            this.levelSelector = levelSelector;
        }
        
        public void Initialize()
        {
            gameSaveData = saveService.Load();
            if (gameSaveData == null)
            {
                gameSaveData = new GameSaveData();
                saveService.Save(gameSaveData);
            }

            Application.targetFrameRate = gameConfig.targetFrameRate;
            lastPlayedLevelIndex = new int[LastPlayedLevelArrCount];
            levelService.SetCurrentLevel(gameSaveData.levelIndex);
            signalBus.Subscribe<LevelStateChangedSignal>(OnLevelStateChanged);
            
            StartGame();
        }
        
        public void StartGame()
        {
            levelService.LoadLevelAsync().Forget();
        }

        public void Dispose()
        {
            signalBus.TryUnsubscribe<LevelStateChangedSignal>(OnLevelStateChanged);
        }
        
        private void OnLevelStateChanged(LevelStateChangedSignal args)
        {
            if (args.State == LevelState.Completed)
            {
                gameSaveData.levelIndex = levelSelector.GetNextLevel(gameSaveData); 
                if (gameSaveData.levelIndex + 1 == gameConfig.totalLevels)
                {
                    gameSaveData.hasPlayedAllLevels = true;
                }
                levelService.SetCurrentLevel(gameSaveData.levelIndex);
                saveService.Save(gameSaveData);
            }
        }
    }
}
