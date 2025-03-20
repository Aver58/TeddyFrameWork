using AIMiniGame.Scripts.Bussiness.View;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

namespace AIMiniGame.Scripts.Bussiness.Controller {
    public class TetrisController : MonoBehaviour {
        public TetrisView view;
        public float fallInterval = 1f;  // 下落间隔时间
        private float fallTimer = 0f;
        private bool gameStarted = false;

        void Start()
        {
            // 初始化游戏
            TetrisModel.Instance.SpawnNewTetromino();
            view.DrawCurrentTetromino();
            gameStarted = true;
        }

        void Update()
        {
            if (!gameStarted) return;

            HandleInput();
            HandleFall();
        }

        void HandleInput()
        {
            // 简单示例：左右移动和旋转
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveCurrentTetromino(new Vector2Int(-1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveCurrentTetromino(new Vector2Int(1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                RotateCurrentTetromino();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                // 快速下落
                MoveCurrentTetromino(new Vector2Int(0, -1));
            }
        }

        void HandleFall()
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= fallInterval)
            {
                fallTimer = 0f;
                // 每个间隔让方块下落1格
                MoveCurrentTetromino(new Vector2Int(0, -1));
            }
        }

        void MoveCurrentTetromino(Vector2Int translation)
        {
            // 模拟移动：直接修改模型中当前方块位置（未检测碰撞，简化处理）
            TetrisModel.Instance.currentPosition += translation;
            view.DrawCurrentTetromino();
        }

        void RotateCurrentTetromino()
        {
            // 简单旋转：增加旋转状态
            TetrisModel.Instance.currentRotation++;
            view.DrawCurrentTetromino();
        }
    }
}