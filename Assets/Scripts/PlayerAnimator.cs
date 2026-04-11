using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string SPEED = "Speed";
    private const string TOOL_TYPE = "ToolType";
    private const string DO_ACTION = "DoAction";

    [SerializeField] private Player player;

    private Animator animator;
    private Transform indicatorTransform;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player.OnToolUsed += Player_OnToolUsed;
        indicatorTransform = GameObject.FindAnyObjectByType<PlayerIndicator>().transform;
    }

    private void Player_OnToolUsed(object sender, Player.OnToolUsedEventArgs e)
    {
        if (indicatorTransform != null)
        {
            // TẠI ĐÂY: Cộng thêm một khoảng offset Y (ví dụ 0.5f) vào vị trí của Player
            // để đưa "điểm gốc tính toán" từ chân lên bụng/ngực mèo.
            Vector3 playerCenter = player.transform.position + new Vector3(0, 0.5f, 0);

            Vector3 direction = (indicatorTransform.position - playerCenter).normalized;

            // Ép các giá trị nhỏ về 0 để tránh việc bị chéo 45 độ
            float animX = direction.x;
            float animY = direction.y;

            // Nếu hướng ngang (X) mạnh hơn hướng dọc (Y), hãy ưu tiên X
            if (Mathf.Abs(animX) > Mathf.Abs(animY)) animY = 0;
            else animX = 0;

            animator.SetFloat(HORIZONTAL, animX);
            animator.SetFloat(VERTICAL, animY);
        }

        UseToolAnimation();
    }

    private void Update()
    {
        Vector2 input = player.GetInputVector();
        // Nếu Player đang chạy, xoay theo hướng chạy
        // Nếu đứng yên (Idle), KHÔNG làm gì cả -> Player sẽ giữ nguyên hướng nhìn cũ
        if (player.IsMoving())
        {
            animator.SetFloat(HORIZONTAL, input.x);
            animator.SetFloat(VERTICAL, input.y);
        }

        animator.SetFloat(SPEED, input.sqrMagnitude);
    }

    public void UseToolAnimation()
    {
        // Cập nhật ToolType để Animator biết chơi animation của Axe, Hoe hay None
        animator.SetInteger(TOOL_TYPE, (int)player.GetEquippedToolType());
        animator.SetTrigger(DO_ACTION);
    }
}
