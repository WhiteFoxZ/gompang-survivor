using UnityEngine;
using System.Collections;

/// <summary>
/// BossPattern 클래스 - 보스의 특수 공격 패턴을 관리합니다.
/// </summary>
public class BossPattern : MonoBehaviour
{
    // 플레이어 위치 참조 (추적 대상)
    Transform _player;

    // 보스 패턴 타이밍 변수
    public float waitTime = 5f;  // 돌진 전 준비 시간 (기 모으기)
    public float dashSpeed = 10f;  // 돌진 속도
    public float dashDuration = 0.2f; // 돌진 지속 시간
    public float waitEndTime = 3f;  // 돌진 전 준비 시간 (기 모으기)


    // 물리 컴포넌트 참조
    private Rigidbody2D rb;

    // 보스 상태 변수
    public bool isDashing = false; // 보스가 현재 돌진 중인지 여부 (다른 클래스에서 참조)

    // 시각 효과 오브젝트 (돌진 지시자)
    public GameObject _LaserPivot;

    /// <summary>
    /// 초기화 - Rigidbody2D 컴포넌트 가져오기
    /// </summary>
    void Start()
    {
        // Rigidbody2D 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            // Rigidbody2D가 없으면 오류 로그 출력
            Debug.LogError("BossPattern: Rigidbody2D 컴포넌트가 없습니다. 추가해주세요.");
        }
    }

    /// <summary>
    /// 오브젝트 활성화 시 호출 - 플레이어 참조 설정 및 시각 효과 초기화
    /// </summary>
    void OnEnable()
    {
        // prefab에서 생성될 때 플레이어의 Transform을 찾아서 할당
        // OnEnable에서만 찾음 (활성화될 때마다 최신 플레이어 위치 참조)
        if (GameManager.instance.player != null)
            _player = GameManager.instance.player.GetComponent<Rigidbody2D>().transform;

        // 시각 효과 오브젝트 inicialmente 비활성화
        _LaserPivot.SetActive(false);
    }

    /// <summary>
    /// 보스 돌진 패턴 코루틴 - 준비 -> 돌진 -> 쿨다운 순서로 진행
    /// </summary>
    /// <returns>코루틴 엔umerator</returns>
    public IEnumerator DashRoutine()
    {
        // 돌진 상태 시작
        isDashing = true;

        // 1. 잠시 멈춤 (기 모으기) - 플레이어를 타겟팅
        Debug.Log("보스가 타겟팅 중...");

        //_LaserPivot 방향은 플레이어방향으로 회전시키기
        Vector2 toPlayer = _player.position - _LaserPivot.transform.position;
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        _LaserPivot.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 돌진 지시자 시각 효과 활성화
        _LaserPivot.SetActive(true);

        // 돌진 방향 계산 (플레이어 방향으로 정규화된 벡터)
        Vector2 direction = (_player.position - transform.position).normalized;

        // 준비 시간 대기
        yield return new WaitForSeconds(waitTime);

        // 2. 플레이어를 향해 돌진 (물리 기반 이동) - 플레이어와 충돌 시 돌진 중단
        Debug.Log("보스가 돌진 시작!");

        _LaserPivot.SetActive(false); // 준비가 끝나면 시각 효과 비활성화

        float elapsed = 0f;
        // 돌진 지속 시간 동안 또는 돌진이 중지될 때까지 반복
        while (elapsed < dashDuration && rb != null && isDashing)
        {
            // 물리 기반 이동 - Rigidbody를 사용하여 위치 업데이트 (충돌 감지 가능)
            rb.MovePosition(rb.position + (Vector2)(direction * dashSpeed * Time.fixedDeltaTime)); // 위치 이동

            Vector2 playerV = (_player.position - transform.position).normalized;

            float dot = Vector2.Dot(direction, playerV);

            // this.Log($" --------------- 보스와 플레이어 간의 방향 dot : {dot}");

            if (dot < -0.2f)
            {
                // this.Log($" 보스와 플레이어 간의 방향 dot : {dot} < 0f, 돌진 중지 조건 충족 ");
                isDashing = false; // 돌진 중지
                // 즉시 속도 0으로 설정하여 밀림 방지
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    this.Log($" 즉시 속도 0으로 설정하여 밀림 방지 : {rb.linearVelocity}  ");

                }
                break; // 루프 탈출
            }


            // 외부에서 돌진 중지 신호가 왔으면 루프 즉시 탈출 (플레이어 충돌 등)
            if (isDashing == false)
            {
                this.Log("보스가 돌진을 멈췄습니다. 충돌 감지됨. 루프 탈출.");
                // 즉시 속도 0으로 설정하여 밀림 방지
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    this.Log($" 보스가 돌진을 멈췄습니다. 충돌 감지됨 즉시 속도 0으로 설정하여 밀림 방지 : {rb.linearVelocity}  ");
                }
                break;
            }

            // 시간 누적 및 다음 물리 업데이트 대기
            elapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 돌진 종료 로그
        Debug.Log("보스가 돌진 종료!");

        // 물리의 속도 초기화 (관성 제거)

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            this.Log($"보스 돌진 종료 후 속도 초기화 : {rb.linearVelocity}  ");
            isDashing = false; // 돌진 상태 종료
        }

        // 3. 돌진이 끝난 후 잠시 멈춤 (쿨다운) 2초 대기
        yield return new WaitForSeconds(waitEndTime);

    }


}