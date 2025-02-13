using UnityEngine;

public class Roll : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovement pm;

    [Header("Rolling")]
    public KeyCode rollKey = KeyCode.LeftControl;
    public float rollYscale = 0.5f;
    public float rollSpeed;
    public float maxRollTime;
    private float rollTimer;

    private bool rolling;
    private float rollCooldown = 2f; // Время кулдауна между подкатами
    private float cooldownTimer;

    private void Start()
    {
        rollTimer = maxRollTime;
        cooldownTimer = 0f; // Инициализация кулдауна
    }

    private void Update()
    {
        StateMachine();
        if (rolling) RollingMovement();

        // Обновляем таймер кулдауна
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        Debug.Log($"Input: {Input.GetKey(rollKey)}, IsGrounded: {pm.isGrounded}, MoveSpeed: {pm.moveSpeed}, SprintSpeed: {pm.sprintSpeed}, Rolling: {rolling}, RollTimer: {rollTimer}, CooldownTimer: {cooldownTimer}");
    }

    private void StateMachine()
    {
        if (Input.GetKey(rollKey) && pm.isGrounded && pm.moveSpeed == pm.sprintSpeed)
        {
            if (!rolling && cooldownTimer <= 0) StartRolling();
            if (rollTimer > 0) rollTimer -= Time.deltaTime;
            if (rollTimer < 0) StopRolling();
        }
        else
        {
            if (rolling) StopRolling();
        }
    }

    private void StartRolling()
    {
        rolling = true;
        transform.localScale = new Vector3(transform.localScale.x, rollYscale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // Запускаем кулдаун
        cooldownTimer = rollCooldown;
    }

    private void RollingMovement()
    {
        rb.AddForce(pm.moveDirection * rollSpeed, ForceMode.Impulse);
    }

    private void StopRolling()
    {
        transform.localScale = new Vector3(transform.localScale.x, pm.startYScale, transform.localScale.z);
        rolling = false;
        rollTimer = maxRollTime;
    }
}
