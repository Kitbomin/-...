using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 따라갈 플레이어
    public Vector3 offset = new Vector3(0f, 1f, -10f); // 카메라 위치 조정
    public float smoothSpeed = 5f; // 카메라 이동 속도

    void LateUpdate()
    {
        if (player == null) return; // 플레이어가 없으면 실행 X

        // 목표 위치 설정 (x, y는 플레이어 + offset, z는 고정)
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, -10f) + offset;
        
        // 부드럽게 이동 (Lerp 사용)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
