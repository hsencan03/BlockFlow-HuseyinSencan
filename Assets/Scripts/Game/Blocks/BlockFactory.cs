using Game.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Game.Blocks
{
    public class BlockFactory
    {
        private readonly GridPrefabsConfig gridPrefabsConfig;
        private readonly DiContainer container;

        public BlockFactory(GridPrefabsConfig gridPrefabsConfig, DiContainer container)
        {
            this.gridPrefabsConfig = gridPrefabsConfig;
            this.container = container;
        }
        
        public GameObject Create(BlockModel blockModel, Vector3 position, BlockRotation rotation, Transform parent)
        {
            BlockView prefab = GetBlockPrefab(blockModel.Type);
            if (prefab == null)
            {
                Debug.LogWarning($"No prefab found for block type: {blockModel.Type}");
                return null;
            }
            
            GameObject blockGo = container.InstantiatePrefab(prefab.gameObject, position, Quaternion.Euler(90, (int)rotation, 0), parent);
            if (blockGo.TryGetComponent<BlockView>(out var blockView))
            {
                blockView.SetData(blockModel);
            }
            
            return blockGo;
        }
        
        private BlockView GetBlockPrefab(BlockType type)
        {
            return type switch
            {
                BlockType.Cube => gridPrefabsConfig.SM_Block_Cube_S_01,
                BlockType.L => gridPrefabsConfig.SM_Block_L_L_01,
                BlockType.LLine => gridPrefabsConfig.SM_Block_Line_L_01,
                BlockType.L_S => gridPrefabsConfig.SM_Block_L_S_01,
                BlockType.Plus => gridPrefabsConfig.SM_Block_Plus_L_01,
                BlockType.T => gridPrefabsConfig.SM_Block_T_L_01,
                BlockType.Z => gridPrefabsConfig.SM_Block_Z_L_01,
                _ => null
            };
        }
    }
}