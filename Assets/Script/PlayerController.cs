using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastPosition;
    private SpriteRenderer spriteRenderer;
    public Sprite PlayerDie;  // 죽었을 때 표시될 스프라이트

    public bool IsMoving { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
        DetectMovement();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed; // linearVelocity → velocity로 수정
    }

    void DetectMovement()
    {
        IsMoving = (rb.linearVelocity.magnitude > 0.1f);
        lastPosition = transform.position;
    }

    public void Die()
    {
        Debug.Log("플레이어 즉사!");

        // 죽었을 때 스프라이트 변경
        if (spriteRenderer != null && PlayerDie != null)
        {
            spriteRenderer.sprite = PlayerDie;
        }

        // 게임 멈추기 (즉시 일시정지)
        Time.timeScale = 0f;  // 게임 일시정지
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EyeMonster"))
        {
            EyeMonster monster = collision.GetComponent<EyeMonster>();

            if (monster != null && monster.currentState == EyeMonster.EyeState.Closed && monster.canBeKilled)
            {
                Debug.Log("눈깔 괴물 처치!");
                Destroy(monster.gameObject);
            }
        }
    }
}
