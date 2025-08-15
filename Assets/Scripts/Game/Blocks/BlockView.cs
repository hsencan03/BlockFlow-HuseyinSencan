using Core.Signals;
using Game.Blocks.Movement;
using Game.Interfaces;
using Game.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Blocks
{
    public class BlockView : MonoBehaviour
    {
        public BlockModel BlockModel => blockModel;
        public BlockConfig BlockConfig => blockConfig;

        [SerializeField] private TMP_Text grindAttemptCounter;
        [SerializeField] private Image arrowImage;
        
        private IColorProvider<BlockColor> colorProvider;
        private BlockConfig blockConfig;
        private SignalBus signalBus;
        
        private BlockModel blockModel;
        private new Renderer renderer;
        private BlockMovement blockMovement;
        
        private Material defaultMaterial;
        private Material frozenMaterial;

        [Inject]
        private void Construct(SignalBus signalBus, BlockConfig blockConfig, IColorProvider<BlockColor> colorProvider, GridPrefabsConfig prefabsConfig)
        {
            this.signalBus = signalBus;
            this.blockConfig = blockConfig;
            this.colorProvider = colorProvider;
            frozenMaterial = prefabsConfig.frozenMaterial;
        }
        
        private void Awake()
        {
            renderer = GetComponentInChildren<Renderer>();
            blockMovement = GetComponent<BlockMovement>();
        }

        private void OnEnable() => signalBus.Subscribe<BlockDestroyedSignal>(OnBlockDestroyed);
        private void OnDisable() => signalBus.TryUnsubscribe<BlockDestroyedSignal>(OnBlockDestroyed);
        
        public void SetData(BlockModel blockModel)
        {
            this.blockModel = blockModel;
            SetBlockViewSettings();
        }

        private void SetBlockViewSettings()
        {
            UpdateColor();
            if (blockModel.State == BlockState.Frozen)
            {
                SetFrozenSettings();
                return;
            }
            if (blockModel.MovementType != BlockMovementType.Free)
            {
                arrowImage.gameObject.SetActive(true);
            }
        }

        private void OnBlockDestroyed(BlockDestroyedSignal args)
        {
            if(args.Block == blockModel)
                return;

            if (blockModel.State == BlockState.Frozen)
            {
                blockModel.OnBlockGrind();
                grindAttemptCounter.SetText(blockModel.GrindAttempts.ToString());
                if (blockModel.State == BlockState.Normal)
                {
                    if (blockModel.MovementType != BlockMovementType.Free)
                    {
                        arrowImage.gameObject.SetActive(true);
                    }
                    grindAttemptCounter.gameObject.SetActive(false);
                    blockMovement.SetMovementStrategy();
                    renderer.material = defaultMaterial;
                    signalBus.TryUnsubscribe<BlockDestroyedSignal>(OnBlockDestroyed);
                }
            }
        }
        
        private void SetFrozenSettings()
        {
            defaultMaterial = renderer.material;
            renderer.material = frozenMaterial;
            SetFrozenUI();
        }

        private void SetFrozenUI()
        {
            grindAttemptCounter.gameObject.SetActive(true);
            grindAttemptCounter.SetText(blockModel.GrindAttempts.ToString());
            Vector3 currentPos = grindAttemptCounter.transform.position;
            Vector3 cameraPos = -Camera.main.transform.position;
            Vector3 direction = new Vector3(0, cameraPos.y - currentPos.y, 0);
            if (direction.sqrMagnitude > 0f)
            {
                grindAttemptCounter.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        private void UpdateColor()
        {
            Color color = colorProvider.GetColor(blockModel.Color);
            renderer.material.color = color;
        }

        public void Destroy()
        {
            blockModel.DestroyObject();
        }
    }
}