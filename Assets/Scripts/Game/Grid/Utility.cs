using System;
using Game.Blocks;
using UnityEngine;

namespace Game.Grid
{
    public static class Utility
    {
        private static Transform gridParent;
        private static int width;
        private static int height;
        private static float cellSize = 1.0f;
        
        public static void Initialize(Transform parent, int gridWidth, int gridHeight, float cellSizeParam = 1.0f)
        {
            gridParent = parent;
            width = gridWidth;
            height = gridHeight;
            cellSize = cellSizeParam;
        }
        
        public static class Shapes
        {
            public static (int xSize, int ySize) GetBlockSpan(BlockType blockType, BlockRotation rotation)
            {
                var offsets = GetSize(blockType, rotation);
                int minXOffset = 0, maxXOffset = 0;
                int minYOffset = 0, maxYOffset = 0;

                foreach (var (xOffset, yOffset) in offsets)
                {
                    minXOffset = Mathf.Min(minXOffset, xOffset);
                    maxXOffset = Mathf.Max(maxXOffset, xOffset);
                    minYOffset = Mathf.Min(minYOffset, yOffset);
                    maxYOffset = Mathf.Max(maxYOffset, yOffset);
                }

                int xSize = maxXOffset - minXOffset + 1;
                int ySize = maxYOffset - minYOffset + 1;

                return (xSize, ySize);
            }
            
            public static Vector3 GetOffset(BlockType type, BlockRotation rotation)
            {
                Vector3 offset = type switch
                {
                    BlockType.L => new Vector3(-0.5f, 1, 0),
                    BlockType.L_S => new Vector3(-0.5f, 1, 0.5f),
                    BlockType.T => new Vector3(0, 1, 0.5f),
                    BlockType.Z => new Vector3(-0.5f, 1, 0),
                    _ => Vector3.up
                };

                float angle = (int)rotation;
                Quaternion rot = Quaternion.Euler(0, angle, 0);
                return rot * offset;
            }

            public static (int x, int y)[] GetSize(BlockType type, BlockRotation rotation)
            {
                (int x, int y)[] baseCoords = type switch
                {
                    BlockType.Cube => new[] { (0, 0) },
                    BlockType.L => new[] { (0, 1), (0, 0), (0, -1), (-1, -1) },
                    BlockType.L_S => new[] { (0, 1), (0, 0), (-1, 0) },
                    BlockType.LLine => new[] { (-1, 0), (0, 0), (1, 0) },
                    BlockType.Plus => new[] { (0, 0), (0, 1), (0, -1), (-1, 0), (1, 0) },
                    BlockType.T => new[] { (-1, 0), (0, 0), (1, 0), (0, 1) },
                    BlockType.Z => new[] { (-1, 1), (0, 1), (0, 0), (1, 0) },
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };

                int steps = rotation switch
                {
                    BlockRotation.Rotation0 => 0,
                    BlockRotation.Rotation90 => 1,
                    BlockRotation.Rotation180 => 2,
                    BlockRotation.Rotation270 => 3,
                    _ => 0
                };

                if (steps == 0)
                    return baseCoords;

                var rotated = new (int x, int y)[baseCoords.Length];

                for (int i = 0; i < baseCoords.Length; i++)
                {
                    int x = baseCoords[i].x;
                    int y = baseCoords[i].y;

                    for (int s = 0; s < steps; s++)
                    {      
                        int temp = x;
                        x = y;
                        y = -temp;
                    }

                    rotated[i] = (x, y);
                }

                return rotated;
            }
        }
        
        public static (int width, int height) GetSize()
        {
            return (width, height);
        }

        public static Vector3 GridToWorldPosition(GridPosition pos)
        {
            float offsetX = (width - 1) * 0.5f * cellSize;
            float offsetZ = (height - 1) * 0.5f * cellSize;

            float xPos = (pos.X * cellSize) - offsetX;
            float zPos = (pos.Y * cellSize) - offsetZ;

            return gridParent.position + new Vector3(xPos, 0f, zPos);
        }
        
        public static GridPosition WorldToGridPosition(Vector3 worldPos) 
        {
            float offsetX = (width - 1) * 0.5f * cellSize;
            float offsetZ = (height - 1) * 0.5f * cellSize;

            float localX = worldPos.x + offsetX;
            float localZ = worldPos.z + offsetZ;

            int x = Mathf.RoundToInt(localX / cellSize);
            int y = Mathf.RoundToInt(localZ / cellSize);

            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);

            return new GridPosition(x, y);
        }

        public static Vector3 GetClosestTileWorldPosition(Vector3 wordPos, BlockType type, BlockRotation rotation)
        {
            Vector3 blockPosition = wordPos - Shapes.GetOffset(type, rotation);
            GridPosition gridPos = GetClosestTilePosition(blockPosition);
            return GridToWorldPosition(gridPos) + Shapes.GetOffset(type, rotation);
        }

        public static GridPosition GetClosestTilePosition(Vector3 worldPos)
        {
            Vector3 gridOrigin = gridParent.position;
            float offsetX = (width - 1) * 0.5f * cellSize;
            float offsetZ = (height - 1) * 0.5f * cellSize;

            float localX = worldPos.x - (gridOrigin.x - offsetX);
            float localZ = worldPos.z - (gridOrigin.z - offsetZ);

            int x = Mathf.RoundToInt(localX / cellSize);
            int y = Mathf.RoundToInt(localZ / cellSize);

            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);

            return new GridPosition(x, y);
        }
        
        public static GridPosition GetClosestTilePosition(Vector3 worldPos, BlockType type, BlockRotation rotation)
        {
            Vector3 offset = Shapes.GetOffset(type, rotation);
            Vector3 adjustedPos = worldPos - offset;
            return GetClosestTilePosition(adjustedPos);
        }
    }
}