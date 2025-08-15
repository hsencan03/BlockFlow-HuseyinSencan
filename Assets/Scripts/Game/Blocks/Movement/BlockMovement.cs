using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Grid;
using Game.Grinders;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Blocks.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(BlockView), typeof(BlockCollisionHandler))]
    public class BlockMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsDragging => isDragging;
        
        [Inject] private IGridValidator gridValidator;
        
        private Vector3 dragOffset;
        private Vector3 originalPosition;
        private Camera mainCamera;
        private bool isDragging;
        private Vector3 newPosition;

        private int draggingFingerId = -1;
        
        private BlockView view;
        private Rigidbody body;
        
        private IMovementStrategy movementStrategy;
        
        private Vector3 movementOffset; 
        
        private float maxVelocity;
        
        private void Start()
        {
            mainCamera = Camera.main;
            view = GetComponent<BlockView>();
            body = GetComponent<Rigidbody>();
            maxVelocity = view.BlockConfig.maxVelocity;
            
            SetMovementStrategy();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            originalPosition = transform.position;
            Vector3 worldPoint = ScreenToWorld(eventData.position);
            dragOffset = transform.position - worldPoint;

            isDragging = true;
            draggingFingerId = eventData.pointerId;
            transform.position += movementOffset;
            ToggleKinematic(false);
        }

        private void Update()
        {
            Vector3 pointerScreenPos = GetPointerScreenPosition();
            Vector3 worldPoint = ScreenToWorld(pointerScreenPos);
            newPosition = movementStrategy.ApplyMovement(transform.position, worldPoint + dragOffset);
            newPosition.y = originalPosition.y;
        }

        private Vector3 GetPointerScreenPosition()
        {
            Vector3 pointerScreenPos = Vector3.zero;
            if (draggingFingerId >= 0 && Input.touchCount > 0)
            {
                Touch? currentTouch = null;
                foreach (Touch touch in Input.touches)
                {
                    if (touch.fingerId == draggingFingerId)
                    {
                        currentTouch = touch;
                        break;
                    }
                }

                if (currentTouch.HasValue)
                {
                    pointerScreenPos = currentTouch.Value.position;
                }
                else
                {
                    isDragging = false;
                    draggingFingerId = -1;
                }
            }
#if UNITY_EDITOR
            else if (Input.GetMouseButton(0))
            {
                pointerScreenPos = Input.mousePosition;
            }
#endif
            return pointerScreenPos;
        }

        private void FixedUpdate()
        {
            if(!isDragging)
                return;
            
            Vector3 velocity = (newPosition - transform.position) / Time.fixedDeltaTime;
            velocity.y = 0; 
            velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
            body.velocity = velocity;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            if (eventData.pointerId != draggingFingerId)
                return; 

            isDragging = false;
            draggingFingerId = -1;
            ToggleKinematic(true);

            TrySnapGridPosition();
        }
        
        public UniTask HandleGrinderCollision(GrinderModel grinderModel, float destroyAnimDuration)
        {
            isDragging = false;
            ToggleKinematic(true);
            TrySnapGridPosition();
            return MoveToGrinder(grinderModel, destroyAnimDuration);
        }
        
        public void SetMovementStrategy()
        {
            movementStrategy = new MovementStrategyFactory().Create(view.BlockModel);
            movementOffset = movementStrategy.GetMovementOffset();
        }

        private Vector3 ScreenToWorld(Vector2 screenPos)
        {
            Vector3 screenPointWithZ = new Vector3(screenPos.x, screenPos.y, mainCamera.WorldToScreenPoint(transform.position).z);
            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPointWithZ);
            return worldPoint;
        }

        private Vector3 SnapToGrid(Vector3 position)
        {
            return Utility.GetClosestTileWorldPosition(position, view.BlockModel.Type, view.BlockModel.Rotation);
        }

        private void TrySnapGridPosition()
        {
            Vector3 snappedPosition = SnapToGrid(transform.position);
            transform.position = snappedPosition;
            view.BlockModel.SetPosition(Utility.GetClosestTilePosition(snappedPosition));

        }
        
        private void ToggleKinematic(bool isKinematic)
        {
            body.isKinematic = isKinematic;
            body.collisionDetectionMode = isKinematic ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
        }

        public UniTask MoveToGrinder(GrinderModel grinder, float duration)
        {
            var blockSpan = Utility.Shapes.GetBlockSpan(view.BlockModel.Type, view.BlockModel.Rotation);
            GridPosition blockPos = view.BlockModel.Position;

            bool isVertical;
            GridPosition grinderStart = grinder.Positions[0];
            if (grinder.Positions.Count > 1)
            {
                GridPosition grinderEnd = grinder.Positions[^1];
                isVertical = grinderStart.X == grinderEnd.X;
            }
            else
            {
                (int width, int height) = Utility.GetSize();
                int x = grinderStart.X;
                int y = grinderStart.Y;
                bool isCorner = (x == 0 && y == 0) || (x == 0 && y == height - 1)
                                                   || (x == width - 1 && y == 0) || (x == width - 1 && y == height - 1);
                
                isVertical = isCorner ? grinder.IsVertical : grinderStart.X == 0 || grinderStart.X == width - 1;
            }

            int delta = isVertical ? (grinderStart.X > blockPos.X ? 1 : -1)  : (grinderStart.Y > blockPos.Y ? 1 : -1);
            
            Vector3 moveVector = isVertical
                ? new Vector3(delta * blockSpan.xSize, 0, 0)
                : new Vector3(0, 0, delta * blockSpan.ySize);

            Vector3 targetPosition = transform.position + moveVector;
            Tweener tween = transform.DOMove(targetPosition, duration).SetEase(Ease.InOutSine);
            return tween.AsyncWaitForCompletion().AsUniTask();
        }
    }
}
