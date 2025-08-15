using Core.Services.TimerService;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.UI.Panel
{
    public class InGamePanel : PanelBehaviour
    {
        [SerializeField] private TMP_Text timerText;

        private ITimerService timerService;
        private Timer levelTimer;
        
        [Inject]
        private void Construct(ITimerService timerService)
        {
            this.timerService = timerService;
        }

        public override async UniTask Show()
        {
            await base.Show();
            levelTimer.Start();
        }

        private void OnEnable()
        {
            levelTimer = timerService.GetTimer(Game.Constants.LevelTimerID);
            UpdateTimerText();
            levelTimer.OnTick += UpdateTimerText;
        }

        private void OnDisable()
        {
            levelTimer.OnTick -= UpdateTimerText;
        }

        private void UpdateTimerText()
        {
            timerText.text = levelTimer.RemainingTime.ToString(@"mm\:ss");
        }
    }
}