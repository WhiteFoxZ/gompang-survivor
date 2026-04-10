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
        // 2D 게임에서는 Z축만 회전하거나 스프라이트를 뒤집어야 합니다
        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        yield return new WaitForSeconds(waitTime);

        // 2. 플레이어를 향해 돌진
        Debug.Log("보스가 돌진 시작!");
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position += (Vector3)(direction * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;


        }
        Debug.Log("보스가 돌진 종료!");
        // 3. 돌진이 끝난 후 잠시 멈춤 (쿨다운)
        yield return new WaitForSeconds(1f);
    }

}
