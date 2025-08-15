using Game.Blocks;
using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game/Config/GridPrefabsConfig")]
    public class GridPrefabsConfig : ScriptableObject
    {
        public GameObject gridTilePrefab;
        
        [Space(10)]
        public GameObject grinderPrefab;
        
        [Space(10)]
        public GameObject wallPrefab;
        public GameObject wallCornerPrefab;

        [Space(10)]
        public BlockView SM_Block_Cube_S_01;
        public BlockView SM_Block_Cube_SL_01;
        public BlockView SM_Block_L_L_01;
        public BlockView SM_Block_L_L_Mirror_01;
        public BlockView SM_Block_L_S_01;
        public BlockView SM_Block_Line_L_01;
        public BlockView SM_Block_Line_S_01;
        public BlockView SM_Block_Plus_L_01;
        public BlockView SM_Block_T_L_01;
        public BlockView SM_Block_Z_L_01;

        [Space(10)] 
        public Material frozenMaterial;
    }
}