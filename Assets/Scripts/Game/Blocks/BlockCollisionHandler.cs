using Core.Signals;
using Game.Blocks.Movement;
using Game.Grid;
using Game.Grinders;
using UnityEngine;
using Zenject;

namespace Game.Blocks
{
    [RequireComponent(typeof(Collider), typeof(BlockView), typeof(BlockMovement))]
    public class BlockCollisionHandler : MonoBehaviour
    {
        private BlockView view;
        private BlockMovement blockMovement;
        private new Collider collider;
        
        private IGridValidator gridValidator;
        private SignalBus signalBus; 
        
        private bool isCollidingWithGrinder;
        
        [Inject]
        private void Construct(IGridValidator gridValidator, SignalBus signalBus)
        {
            this.gridValidator = gridValidator;
            this.signalBus = signalBus;
        }
        
        private void Awake()
        {
            view = GetComponent<BlockView>();
            collider = GetComponent<Collider>();
            blockMovement = GetComponent<BlockMovement>();
        }

        private void Update()
        {
            if(!blockMovement.IsDragging || isCollidingWithGrinder)
                return;
            
            var offsets = Utility.Shapes.GetSize(view.BlockModel.Type, view.BlockModel.Rotation);
            GridPosition basePos = Utility.GetClosestTilePosition(transform.position, view.BlockModel.Type, view.BlockModel.Rotation);
            foreach (var offset in offsets)
            {
                GridPosition checkPos = new GridPosition(basePos.X + offset.x, basePos.Y + offset.y);
                if (gridValidator.IsBlockFitsInGrinder(view.BlockModel, basePos, checkPos, out GrinderModel grinderModel))
                {
                    OnCollideWithGrinder(grinderModel);
                    break; 
                }
            }
        }
            
        private async void OnCollideWithGrinder(GrinderModel grinderModel)
        {
            isCollidingWithGrinder = true;
            collider.enabled = false;
            signalBus.AbstractFire(new BlockDestroyedSignal(view.BlockModel, grinderModel, view.BlockConfig.destroyAnimDurationSec));
            await blockMovement.HandleGrinderCollision(grinderModel, view.BlockConfig.destroyAnimDurationSec);
            view.Destroy();
        }
    }
}