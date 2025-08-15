using System.Collections.Generic;
using System.Linq;
using Game.Blocks;
using Game.Data;
using Game.Grinders;
using Game.ScriptableObjects;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Grid
{
    public class GridSystem : IGridValidator
    {
        private readonly Transform gridParent;
        private readonly GridPrefabsConfig gridPrefabs;
        private readonly BlockFactory blockFactory;
        private readonly GrinderFactory grinderFactory;
        private readonly float cellSize = 1f;

        private Tile<BlockModel>[] gridTiles;
        private IList<GrinderModel> grinders;
        private LevelData levelData;

        [Inject]
        public GridSystem([Inject(Id = Constants.GridParentTransformId)] Transform gridParent, GridPrefabsConfig gridPrefabs, BlockFactory blockFactory, GrinderFactory grinderFactory)
        {
            this.gridParent = gridParent;
            this.gridPrefabs = gridPrefabs;
            this.blockFactory = blockFactory;
            this.grinderFactory = grinderFactory;
        }

        public void CreateGridTiles(LevelData levelDataParam)
        {
            ClearGrid();
            levelData = levelDataParam;
            gridTiles = new Tile<BlockModel>[levelData.gridWidth * levelData.gridHeight];
            grinders = new List<GrinderModel>(levelData.grinders.Count);
            
            Utility.Initialize(gridParent, levelData.gridWidth, levelData.gridHeight, cellSize);

            for (int y = 0; y < levelData.gridHeight; y++)
            {
                for(int x = 0; x < levelData.gridWidth; x++)
                {
                    GridPosition position = new GridPosition(x, y);
                    Vector3 pos = Utility.GridToWorldPosition(position);
                    GameObject tile = Object.Instantiate(gridPrefabs.gridTilePrefab, pos, gridPrefabs.gridTilePrefab.transform.rotation, gridParent);
                    tile.name = $"Tile_{x}_{y}";
                    gridTiles[y * levelData.gridWidth + x] = new Tile<BlockModel>(position, tile);
                }
            }

            LoadWalls();
            LoadBlocks();
        }
        
        public bool IsPositionValid(int x, int y) => x >= 0 && x < levelData?.gridWidth && y >= 0 && y < levelData?.gridHeight;

        public bool IsPositionValid(GridPosition pos, BlockType blockType, BlockRotation rotation)
        {
            if (!IsPositionValid(pos.X, pos.Y))
                return false;

            var shapeOffsets = Utility.Shapes.GetSize(blockType, rotation);
            foreach (var (offsetX, offsetY) in shapeOffsets)
            {
                int checkX = pos.X + offsetX;
                int checkY = pos.Y + offsetY;

                if (!IsPositionValid(checkX, checkY) || !gridTiles[checkY * levelData.gridWidth + checkX].IsEmpty)
                    return false;
            }

            return true;
        }
        
        public bool IsPositionValid(Vector3 worldPos, BlockType blockType, BlockRotation rotation)
        {
            GridPosition gridPos = Utility.WorldToGridPosition(worldPos);
            return IsPositionValid(gridPos, blockType, rotation);
        }

        public bool IsBlockFitsInGrinder(BlockModel blockModel, GridPosition blockPosition, GridPosition checkPosition, out GrinderModel grinder)
        {
            grinder = null;
            if (!TryGetGrinder(checkPosition, out GrinderModel grinderModel))
                return false;

            if (blockModel.Color != grinderModel.Color)
                return false;

            var offsets = Utility.Shapes.GetSize(blockModel.Type, blockModel.Rotation);
            GridPosition grinderCorner = grinderModel.Positions.FirstOrDefault();
            
            bool isLeftEdge = grinderCorner.X == 0 || grinderCorner.X == levelData.gridWidth - 1;
            
            bool isCorner = grinderCorner is { X: 0, Y: 0 } || (grinderCorner.X == 0 && grinderCorner.Y == levelData.gridHeight - 1)
                                                            || (grinderCorner.X == levelData.gridWidth - 1 && grinderCorner.Y == 0) ||
                                                            (grinderCorner.X == levelData.gridWidth - 1 && grinderCorner.Y == levelData.gridHeight - 1);

            if (isCorner)
            {
                isLeftEdge = grinderModel.IsVertical;
            }

            foreach (var (xOffset, yOffset) in offsets)
            {
                var position = new GridPosition(blockPosition.X + xOffset, blockPosition.Y + yOffset);

                if (!IsPositionValid(position.X, position.Y))
                {
                    return false;
                }

                if (isLeftEdge && grinderModel.Positions.All(p => p.Y != position.Y))
                {
                    return false;
                }

                if (!isLeftEdge && grinderModel.Positions.All(p => p.X != position.X))
                {
                    return false;
                }
            }

            grinder = grinderModel;
            return true;
        }

        private bool TryGetGrinder(GridPosition position, out GrinderModel grinderModel)
        {
            foreach (GrinderModel grinder in grinders)
            {
                bool isHorizontal = grinder.Positions.All(p => p.Y == grinder.Positions[0].Y);
                bool isVertical = grinder.Positions.All(p => p.X == grinder.Positions[0].X);

                if (isHorizontal && (position.Y == grinder.Positions[0].Y || position.Y == grinder.Positions[0].Y))
                {
                    int minX = grinder.Positions.Min(p => p.X);
                    int maxX = grinder.Positions.Max(p => p.X);
                    if (position.X >= minX && position.X <= maxX)
                    {
                        grinderModel = grinder;
                        return true;
                    }
                }

                if (isVertical && (position.X == grinder.Positions[0].X || position.X == grinder.Positions[0].X))
                {
                    int minY = grinder.Positions.Min(p => p.Y);
                    int maxY = grinder.Positions.Max(p => p.Y);
                    if (position.Y >= minY && position.Y <= maxY)
                    {
                        grinderModel = grinder;
                        return true;
                    }
                }
            }
            grinderModel = null;
            return false;
        }

        public Tile<BlockModel> GetTileAtPosition(int x, int y)
        {
            if (!IsPositionValid(x, y))
                return null;

            return gridTiles[y * levelData.gridWidth + x];
        }
        
        public void SetBlockInGridTiles(GridPosition pivotPos, BlockType type, BlockRotation rotation, [CanBeNull] BlockModel blockModel)
        {
            var shapeOffsets = Utility.Shapes.GetSize(type, rotation);

            foreach (var (offsetX, offsetY) in shapeOffsets)
            {
                GridPosition tilePos = new GridPosition(pivotPos.X + offsetX, pivotPos.Y + offsetY);

                int index = tilePos.Y * levelData.gridWidth + tilePos.X;
                if (index >= 0 && index < gridTiles.Length)
                {
                    gridTiles[index].SetData(blockModel);
                }
            }
        }
        
        private void LoadBlocks()
        {
            foreach (var blockData in levelData.blocks)
            {
                if (IsPositionValid(blockData.x, blockData.y))
                {
                    SpawnBlocks(new GridPosition(blockData.x, blockData.y), blockData);
                }
                else
                {
                    Debug.LogWarning($"Invalid block position {blockData.x},{blockData.y}");
                }
            }
        }

        private void LoadWalls()
        {
            int width = levelData.gridWidth;
            int height = levelData.gridHeight;

            GameObject CreateObject(GameObject prefab, Transform parent, Vector3 localPosition,
                Quaternion localRotation)
            {
                GameObject obj = Object.Instantiate(prefab, parent);
                obj.transform.localPosition = localPosition;
                obj.transform.localRotation = localRotation;
                return obj;
            }

            bool TryGetGrinder(int x, int y, out GrinderData grinderData) => (grinderData = levelData.grinders?.FirstOrDefault(g => g.x == x && g.y == y)) != null;

            void CreateGrinderLine(Transform parent, GrinderData data, bool isVertical, Vector3 basePos,
                Quaternion rotation)
            {
                Vector3 offset = isVertical ? Vector3.up : Vector3.left;
                GrinderModel grinderModel = new GrinderModel(data);
                grinders.Add(grinderModel);
                for (int seg = 0; seg < data.length; seg++)
                {
                    bool isLeftOrBottomEdge = (isVertical && data.x == 0) || (!isVertical && data.y == 0);
                    grinderFactory.Create(grinderModel, parent, basePos + offset * seg, rotation, isLeftOrBottomEdge);
                    
                    int gridX = isVertical ? data.x : data.x + seg;
                    int gridY = isVertical ? data.y + seg : data.y;
                    grinderModel.AddPosition(new GridPosition(gridX, gridY));
                }
            }

            void PlaceLine(int fixedCoord, int rangeStart, int rangeEnd, bool isVertical, Vector3 pos, Quaternion rot)
            {
                for (int i = rangeStart; i < rangeEnd; i++)
                {
                    int x = isVertical ? fixedCoord : i;
                    int y = isVertical ? i : fixedCoord;
                    
                    bool isCorner = (x == 0 && y == 0) || (x == 0 && y == levelData.gridHeight - 1)
                                                       || (x == levelData.gridWidth - 1 && y == 0) || (x == levelData.gridWidth - 1 && y == levelData.gridHeight - 1);

                    Transform tile = GetTileAtPosition(x, y).VisualRepresentation.transform;

                    if (TryGetGrinder(x, y, out GrinderData grinderData) && !(isCorner && isVertical != grinderData.isVertical))
                    {
                        CreateGrinderLine(tile, grinderData, isVertical, pos, rot);
                        i += grinderData.length - 1;
                    }
                    else
                    {
                        CreateObject(gridPrefabs.wallPrefab, tile, pos, rot);
                    }
                }
            }

            void PlaceCorner(int x, int y, Vector3 pos, Quaternion rot)
            {
                Transform tile = GetTileAtPosition(x, y).VisualRepresentation.transform;
                CreateObject(gridPrefabs.wallCornerPrefab, tile, pos, rot);
            }


            PlaceLine(0, 0, height, true, new Vector3(0.8f, -0.5f, 0f), Quaternion.identity); 
            PlaceLine(width - 1, 0, height, true, new Vector3(-0.5f, -0.5f, 0f), Quaternion.identity); 
            PlaceLine(height - 1, 0, width, false, new Vector3(-0.5f, 0.5f, 0), Quaternion.Euler(0f, 0f, -90f)); 
            PlaceLine(0, 0, width, false, new Vector3(-0.5f, -0.8f, 0), Quaternion.Euler(0f, 0f, -90f)); 
            
            PlaceCorner(0, 0, new Vector3(0.5f, -0.5f, 0), Quaternion.identity); 
            PlaceCorner(width - 1, 0, new Vector3(-0.5f, -0.5f, 0), Quaternion.Euler(0f, 0f, 270));
            PlaceCorner(width - 1, height - 1, new Vector3(-0.5f, 0.5f, 0), Quaternion.Euler(0f, 0f, 180)); 
            PlaceCorner(0, height - 1, new Vector3(0.5f, 0.5f, 0), Quaternion.Euler(0, 0, 90));
        }

        private void SpawnBlocks(GridPosition pos, BlockData blockData)
        {
            if (!IsPositionValid(pos, blockData.type, blockData.rotation))
            {
                Debug.LogWarning($"Invalid block position {pos.X},{pos.Y}");
                return;
            }

            Vector3 worldPos = Utility.GridToWorldPosition(pos);
            Vector3 blockPos = worldPos + Utility.Shapes.GetOffset(blockData.type, blockData.rotation);

            BlockModel blockModel = new BlockModel(this, pos, blockData);
            GameObject blockGo = blockFactory.Create(blockModel, blockPos, blockData.rotation, gridParent);
            if (blockGo != null)
            {
                blockModel.SetObject(blockGo);
                SetBlockInGridTiles(pos, blockModel.Type, blockModel.Rotation, blockModel);
                gridTiles[pos.Y * levelData.gridWidth + pos.X].SetData(blockModel);
            }
        }
        
        private void ClearGrid()
        {
            foreach (Transform child in gridParent)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}