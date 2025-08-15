using Game.Booster.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Booster.UI
{
    public class BoosterUIHandler : MonoBehaviour
    {
        [SerializeField] private BoosterSo boosterSo;
        
        [Header("UI"), Space(10)] [SerializeField] private Button button;
        
        [Inject] private IBoosterService boosterService;

        private void Awake() => button.onClick.AddListener(OnBoosterButtonClicked);
        private void Start() => boosterService.RegisterBooster(boosterSo);

        private async void OnBoosterButtonClicked()
        {
            button.interactable = false;
            await boosterService.ActivateBooster(boosterSo);
            button.interactable = true;
        }
    }
}