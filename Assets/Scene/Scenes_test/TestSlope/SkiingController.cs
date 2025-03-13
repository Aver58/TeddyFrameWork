using UnityEngine;

public class SkiingController : MonoBehaviour
{
    public CharacterController characterController;
    public float gravity = 9.8f;
    public float maxSpeed = 20f; // 最大滑行速度
    public float acceleration = 5f; // 滑行加速度
    public float jumpForce = 10f; // 跳跃力
    private Vector3 velocity;
    private bool isGrounded;

    void Update()
    {
        // 地面检测
        isGrounded = characterController.isGrounded;
        if (!isGrounded) {
            Debug.LogError("Not on the ground!");
        }
        if (isGrounded)
        {
            // 获取地面法线
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;

                // 沿坡面方向加速
                velocity += slopeDirection * acceleration * Time.deltaTime;
                velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

                var position = transform.position;
                Debug.DrawLine(position, position + slopeDirection * 2, Color.green);
                Debug.DrawLine(position, position + velocity * 2, Color.blue);

                // 跳跃
                if (Input.GetButtonDown("Jump"))
                {
                    velocity.y = jumpForce;
                }
            }
        }
        else
        {
            // 空中运动
            velocity.y -= gravity * Time.deltaTime;
        }

        // 移动角色
        characterController.Move(velocity * Time.deltaTime);
    }
}
