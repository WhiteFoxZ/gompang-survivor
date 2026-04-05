using UnityEngine;
using Unity.Cinemachine; // Cinemachine 3.x 버전 기준 (구버전은 using Cinemachine;)



public class WallSpawner : MonoBehaviour
{
    public GameObject wallGroup; // 4개의 벽을 담고 있는 부모 오브젝트
    public Transform player;     // 플레이어 위치
    public float range = 5.0f;   // 플레이어로부터 벽까지의 거리

    [Header("카메라설정")]
    public CinemachineCamera vcam; // 하이어라키의 Cinemachine Camera 드래그 앤 드롭

    private Transform originalFollowTarget; // 원래 따라가던 대상 저장용

    void Start()
    {
        // 처음에 벽은 꺼두고, 원래 대상을 저장해둡니다.
        wallGroup.SetActive(false);
        originalFollowTarget = vcam.Follow;
    }


    // 이 함수를 호출하면 벽이 플레이어 주변에 고정됩니다.
    public void SpawnFixedWalls()
    {
        this.Log("************** 벽생성 *************");

        // 시네머신 Follow 대상을 null로 만들어 그 자리에 멈추게 함
        vcam.Follow = null;

        // 1. 벽 묶음을 활성화
        wallGroup.SetActive(true);

        // 2. 현재 플레이어의 위치를 벽 묶음의 위치로 설정 (고정!)
        wallGroup.transform.position = player.position;


    }

    // 벽을 없애고 싶을 때 호출
    public void RemoveWalls()
    {
        this.Log("************** 벽숨김 *************");
        wallGroup.SetActive(false);
        // 다시 플레이어를 따라가도록 설정
        vcam.Follow = originalFollowTarget;
    }
}
