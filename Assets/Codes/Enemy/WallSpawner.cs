using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallGroup; // 4개의 벽을 담고 있는 부모 오브젝트
    public Transform player;     // 플레이어 위치
    public float range = 5.0f;   // 플레이어로부터 벽까지의 거리

    // 이 함수를 호출하면 벽이 플레이어 주변에 고정됩니다.
    public void SpawnFixedWalls()
    {
        // 1. 벽 묶음을 활성화
        wallGroup.SetActive(true);

        // 2. 현재 플레이어의 위치를 벽 묶음의 위치로 설정 (고정!)
        wallGroup.transform.position = player.position;

        // 이제 wallGroup은 플레이어를 따라가지 않고 그 자리에 멈춰있습니다.
    }

    // 벽을 없애고 싶을 때 호출
    public void RemoveWalls()
    {
        wallGroup.SetActive(false);
    }
}
