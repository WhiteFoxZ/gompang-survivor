using UnityEngine;
using UnityEngine.UI;

public class AutoHorizontalStop : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float duration = 20f;       // 총 걸리는 시간 (20초)
    public int targetIndex = 20;       // 멈출 이미지 번호
    public int totalImages = 100;      // 전체 이미지 개수
    public int loopCount = 5;          // 20초 동안 전체를 몇 번 왕복할지 (속도감 결정)

    private float timer = 0f;
    private bool isFinished = true;


    // 버튼에 연결할 함수
    void OnEnable()
    {
        timer = 0f;
        isFinished = false;
        Debug.Log("스크롤 시작!");
    }




    void Update()
    {
        if (isFinished || scrollRect == null) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // 1. 속도감 연출: Ease-Out (처음엔 빠르고 끝에 느려짐)
        // t를 보정하여 초반에 훨씬 더 많이 움직이게 만듭니다.
        float curve = 1f - Mathf.Pow(1f - t, 3f);

        // 2. 전체 이동 거리 계산
        // (전체 한 바퀴 * 루프 횟수) + 최종 목표 위치
        float targetPos = (float)(targetIndex - 1) / (totalImages - 1);
        float totalMovement = loopCount + targetPos;

        // 3. 실제 적용 (NormalizedPosition은 0~1 사이여야 하므로 % 1 사용)
        float currentPos = (curve * totalMovement) % 1.0001f;
        scrollRect.horizontalNormalizedPosition = currentPos;

        if (t >= 1f)
        {
            scrollRect.horizontalNormalizedPosition = targetPos; // 정확한 위치 고정
            isFinished = true;
        }
    }
}
