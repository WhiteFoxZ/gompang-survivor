using UnityEngine;

using System.Collections;


public class BossPattern : MonoBehaviour
{
    Transform _player;      // 플레이어 위치
    public float waitTime = 3f;  // 멈춰 있는 시간
    public float dashSpeed = 10f;  // 돌진 속도
    public float dashDuration = 0.5f; // 돌진 지속 시간
    private Rigidbody2D rb;
    public bool isDashing = false; // 돌진 중인지 여부


    public GameObject _Square;

    void Start()
    {

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("BossPattern: Rigidbody2D 컴포넌트가 없습니다. 추가해주세요.");
        }


    }

    void OnEnable()
    {
        //prefab에서 생성될 때 플레이어 Transform을 찾아 할당 enable 에서만 찾아짐
        if (GameManager.instance.player != null)
            _player = GameManager.instance.player.GetComponent<Rigidbody2D>().transform;


        _Square.SetActive(false);
    }

    public void StartDashPattern()
    {
        if (_player == null)
        {
            Debug.LogError("플레이어 Transform이 할당되지 않았습니다. BossPattern 스크립트에 플레이어 Transform을 할당해주세요.");
        }


        StartCoroutine(DashRoutine());
    }

    public IEnumerator DashRoutine()
    {
        // 1. 잠시 멈춤 (기 모으기)
        Debug.Log("보스가 타겟팅 중...");

        _Square.SetActive(true);

        isDashing = true; // 돌진 상태 시작

        //direction 계산
        Vector2 direction = (_player.position - transform.position).normalized;

        yield return new WaitForSeconds(waitTime);

        // 2. 플레이어를 향해 돌진 (물리 기반 이동) - PLAYER 와 충돌시 돌진을 멈춤
        Debug.Log("보스가 돌진 시작!");
        float elapsed = 0f;
        while (elapsed < dashDuration && rb != null && isDashing)
        {
            // 물리 기반 이동 사용 (충돌 처리됨)
            rb.MovePosition(rb.position + (Vector2)(direction * dashSpeed * Time.fixedDeltaTime));
            elapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("보스가 돌진 종료!");

        _Square.SetActive(false);

        // 물리의 속도 초기화
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        isDashing = false; // 돌진 상태 해제

        // 3. 돌진이 끝난 후 잠시 멈춤 (쿨다운)
        yield return new WaitForSeconds(3);
    }
}