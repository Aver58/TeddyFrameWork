using UnityEngine;

namespace Origins.Entity {
    public class HeroActor : Actor {
        public Entity HeroEntity;
        private Rigidbody2D rigidbody2D;
        private Vector2 inputPos;

        public void Init(HeroEntity heroEntity, Rigidbody2D rigidbody) {
            inputPos = new Vector2();
            rigidbody2D = rigidbody;
            HeroEntity = heroEntity;
        }

        //获取物理操作
        public override void OnUpdate() {
            OnInput();
        }

        private void OnInput() {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            Debug.LogError($"horizontal {horizontal} vertical {vertical}");
            if (horizontal > 0.01f || vertical > 0.01f) {
                inputPos.x = horizontal;
                inputPos.y = vertical;
                //向量归一，使其移动速度一致。
                inputPos.Normalize();
                rigidbody2D.velocity = inputPos * HeroEntity.MoveSpeed;
            }
        }
    }
}