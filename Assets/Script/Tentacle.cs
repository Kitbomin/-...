using System.Collections;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public float attackDelay = 1.5f; // 경고 후 공격까지의 시간
    public float knockbackForce = 5f; // 플레이어 밀쳐내는 힘
    public LayerMask destructibleLayer; // 엄폐물 체크

    private SpriteRenderer spriteRenderer;
    private Collider2D hitCollider;

    public Sprite warningSprite;
    public Sprite attackSprite;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitCollider = GetComponent<Collider2D>();
        hitCollider.enabled = false; // 초기엔 비활성화
    }

    public void StartAttack(float delay)
    {
        attackDelay = delay;
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        // 경고 상태 (범위 표시)
        spriteRenderer.sprite = warningSprite;
        yield return new WaitForSeconds(attackDelay);

        // 실제 공격 상태
        spriteRenderer.sprite = attackSprite;
        hitCollider.enabled = true; // 충돌 판정 활성화
        yield return new WaitForSeconds(0.5f);

        // 촉수 사라짐
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어를 밀쳐냄
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
        else if (((1 << other.gameObject.layer) & destructibleLayer) != 0)
        {
            // 엄폐물을 밀어냄
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 pushDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(pushDir * (knockbackForce * 2f), ForceMode2D.Impulse);
            }
        }
    }
}
