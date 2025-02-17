using System.Collections;
using UnityEngine;

public class EyeMonster : MonoBehaviour
{
    public enum EyeState {Open, Closed}
    public EyeState currentState = EyeState.Closed;

    public Transform scanCenter;
    public float scanRadius = 5f;
    public LayerMask playerLayer;
    public float stillTimeThreshold = 3f;

    private bool isScanning = false;
    private float stillTimer = 0f;
    private PlayerController player;

    public GameObject tentaclePrefab; // 프리팹 할당
    public Transform[] tentacleSpawnPoints; // 촉수 스폰 위치 배열
    public float tentacleWarningTime = 1.5f; // 촉수 공격 전 경고 시간

    public float openDuration = 2f;
    public float closedDuration = 3f;

    private SpriteRenderer spriteRenderer;
    public Sprite openEyeSprite;
    public Sprite closedEyeSprite; 

    public bool canBeKilled = false; //  변수 추가

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(ChangeEyeState());
    }

    void Update()
    {
        ScanForPlayer();
        if (player != null)
        {
            FollowPlayer();
        }
        
    }

    void SpawnTentacle()
    {
        // 무작위 위치에서 촉수 소환
        Transform spawnPoint = tentacleSpawnPoints[Random.Range(0, tentacleSpawnPoints.Length)];
        GameObject tentacle = Instantiate(tentaclePrefab, spawnPoint.position, Quaternion.identity);
        
        // 촉수에게 공격을 시작하라고 전달
        tentacle.GetComponent<Tentacle>().StartAttack(tentacleWarningTime);
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            float targetX = player.transform.position.x;
            Vector2 newPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, newPosition, Time.deltaTime *2f);

        }
    }

    void ScanForPlayer()
    {
        if (currentState == EyeState.Closed) return;

        // 플레이어 탐지
        Collider2D detectedPlayer = Physics2D.OverlapCircle(scanCenter.position, scanRadius, playerLayer);

        if (detectedPlayer)
        {
            player = detectedPlayer.GetComponent<PlayerController>();

            if (player != null)
            {
                // 플레이어와 EyeMonster 사이의 거리와 방향을 계산
                Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

                // Raycast로 장애물 확인 (LayerMask로 장애물만 체크)
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Obstacle"));

                if (hit.collider != null)
                {
                    Debug.Log("장애물이 가로막고 있어 플레이어를 감지할 수 없음");
                    return; // 장애물이 있으면 플레이어를 감지할 수 없음
                }

                // 플레이어가 움직이면 KillPlayer() 실행
                if (player.IsMoving)
                {
                    KillPlayer();
                }
                else
                {
                    stillTimer += Time.deltaTime;
                    if (stillTimer >= stillTimeThreshold)
                    {
                        Debug.Log("통과");
                        stillTimer = 0;
                    }
                }
            }
        }
        else
        {
            stillTimer = 0;
        }
    }


    void KillPlayer()
    {
        Debug.Log("플레이어 죽음 감지됨!");
        Debug.Log("으앙 쥬금");

        if (player != null)
        {
            player.Die();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(scanCenter.position, scanRadius);
    }

    void SummonTentacle()
    {
        Vector2 attackPosition = new Vector2(Random.Range(-4f, 4f), transform.position.y - 1f);
        GameObject tentacle = Instantiate(tentaclePrefab, attackPosition, Quaternion.identity);
    }



    IEnumerator ChangeEyeState()
    {
        while (true)
        {
            currentState = EyeState.Open;
            spriteRenderer.sprite = openEyeSprite;
            Debug.Log("눈깔 괴물이 눈을 떴다!");
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            SummonTentacle();

            currentState = EyeState.Closed;
            spriteRenderer.sprite = closedEyeSprite;
            Debug.Log("눈깔 괴물이 눈을 감았다... 이동 중...");

            transform.position = GetRandomPosition();
            canBeKilled = true; // 눈 감을 때 처치 가능하도록 설정

            yield return new WaitForSeconds(Random.Range(3f, 6f));
            canBeKilled = false; // 다시 처치 불가능하도록 설정
        }
    }

    Vector2 GetRandomPosition()
    {
        float newX = Random.Range(-5f, 5f); // X축만 랜덤 이동
        return new Vector2(newX, transform.position.y); // Y는 고정
    }

    void TryKillMonster()
    {
        if (currentState == EyeState.Closed && canBeKilled)
        {
            Debug.Log("눈깔 괴물 처치 성공!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("눈깔 괴물: 눈을 뜨고 있어서 처치 불가능!");
        }
    }
}
