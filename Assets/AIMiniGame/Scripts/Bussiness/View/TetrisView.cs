using System.Collections.Generic;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

namespace AIMiniGame.Scripts.Bussiness.View {
    public class TetrisView : MonoBehaviour {
        public GameObject blockPrefab; // 指定预制件
        public Transform gridParent;   // 用于承载所有块的父对象
        public float blockSize = 1f;   // 单个块的大小

        private List<GameObject> currentBlocks = new List<GameObject>();

        // 根据模型数据绘制当前方块
        public void DrawCurrentTetromino()
        {
            // 清除之前的块
            foreach (var block in currentBlocks)
            {
                Destroy(block);
            }
            currentBlocks.Clear();

            // 获取当前方块数据
            Vector2Int[] cells = TetrisModel.Instance.GetCurrentCells();
            Vector2Int pos = TetrisModel.Instance.currentPosition;

            // 遍历每个单元格并实例化块
            foreach (var cell in cells)
            {
                // 计算在网格中的绝对位置
                Vector2Int gridPos = pos + cell;
                Vector3 worldPos = new Vector3(gridPos.x * blockSize, gridPos.y * blockSize, 0);

                GameObject block = Instantiate(blockPrefab, worldPos, Quaternion.identity, gridParent);
                currentBlocks.Add(block);
            }
        }
    }
}