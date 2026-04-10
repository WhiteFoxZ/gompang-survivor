using UnityEngine;

using System.Collections;


public class BossPattern : MonoBehaviour
{
    Transform _player;      // 플레이어 위치
    public float waitTime = 1.5f;  // 멈춰 있는 시간
    public float dashSpeed = 30f;  // 돌진 속도
    public float dashDuration = 0.5f; // 돌진 지속 시간

    void Start()
    {
        if (_player == null)
        {
            Debug.LogError("플레이어 Transform이 할당되지 않았습니다. BossPattern 스크립트에 플레이어 Transform을 할당해주세요.");
        }
    }

    void OnEnable()
    {
        if (GameManager.instance.player != null)
            _player = GameManager.instance.player.GetComponent<Rigidbody2D>().transform;

    }

    public void StartDashPattern()
    {
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        // 1. 잠시 멈춤 (기 모으기)
        Debug.Log("보스가 타겟팅 중...");
        // 이때 보스가 플레이어를 쳐다보게 하면 더 위협적입니다.
        transform.LookAt(_player.position);
        yield return new WaitForSeconds(waitTime);

        // 2. 돌진 방향 결정 (멈춘 시점의 플레이어 위치로)
        Vector3 targetDir = (_player.position - transform.position).normalized;
        float elapsed = 0f;

        Debug.Log("보스 돌진!");
        // 3. 정해진 시간 동안 빠르게 이동
        while (elapsed < dashDuration)
        {
            // 벽에 부딪히는 처리를 위해 Translate나 Rigidbody를 권장합니다.
            transform.position += targetDir * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
