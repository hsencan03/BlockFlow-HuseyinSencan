using System;
using Core.Services.LevelLoader;
using Core.Services.TimerService;
using Core.Signals;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Enums;
using Game.Grid;
using UnityEngine;
using Zenject;

namespace Game.Level.LevelService
{
    public class LevelService : ILevelService, IInitializable, IDisposable
    {
        public int CurrentLevel { get; private set; }
        
        private readonly SignalBus signalBus;
        private readonly ILevelLoader levelLoader;
        private readonly ITimerService timerService;
        private readonly GridSystem gridService;
        
        private LevelData levelData;
        
        private int remainingBlockCount;
        private Timer timer;

        public LevelService(SignalBus signalBus, ILevelLoader levelLoader, ITimerService timerService, GridSystem gridService)
        {
            this.signalBus = signalBus;
            this.levelLoader = levelLoader;
            this.timerService = timerService;
            this.gridService = gridService;
        }
        
        public void Initialize()
        {
            signalBus.Subscribe<BlockDestroyedSignal>(OnBlockDestroyed);
        }
        
        public async UniTask LoadLevelAsync()
        {
            levelData = await levelLoader.LoadLevelAsync((CurrentLevel + 1).ToString());

            if (levelData == null)
            {
                Debug.LogError($"Level {CurrentLevel} data not found!");
                return;
            }
            
            remainingBlockCount = levelData.blocks.Count;
            gridService.CreateGridTiles(levelData);
            CreateLevelTimer();
            await UniTask.Yield();
            signalBus.AbstractFire(new LevelStateChangedSignal(CurrentLevel, LevelState.Playing));
        }

        private void CreateLevelTimer()
        {
            TimerParams timerParams = new TimerParams(Game.Constants.LevelTimerID,  
                TimeSpan.ParseExact(levelData.timeLimit, "mm\\:ss", null), false);
            timer = timerService.Create(timerParams);
            timer.OnTimerCompleted += OnTimerCompleted;
        }

        public void SetCurrentLevel(int levelIndex)
        {
            CurrentLevel = levelIndex;
        }

        public void Dispose()
        {
            signalBus.TryUnsubscribe<BlockDestroyedSignal>(OnBlockDestroyed);
        }

        private async void OnBlockDestroyed(BlockDestroyedSignal args)
        {
            remainingBlockCount--;
            if (remainingBlockCount <= 0)
            {
                DisposeTimer();
                await UniTask.Delay(TimeSpan.FromSeconds(args.AnimDurationSec));
                signalBus.AbstractFire(new LevelStateChangedSignal(CurrentLevel, LevelState.Completed));
            }
        }

        private void OnTimerCompleted()
        {
            DisposeTimer();
            signalBus.AbstractFire(new LevelStateChangedSignal(CurrentLevel, LevelState.GameOver));
        }
        
        private void DisposeTimer()
        {
            timer.OnTimerCompleted -= OnTimerCompleted;
            timerService.DisposeTimer(Game.Constants.LevelTimerID);
        }
    }
}