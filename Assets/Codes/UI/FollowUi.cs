using UnityEngine;

/// <summary>
/// 따라다니는 UI 클래스 - 오브젝트의 위치에 따라 UI를 이동시킵니다.
/// </summary>
public class FollowUi : MonoBehaviour
{

    [Header("Health,HealthSlider 포지션 변경하지 마세요")]
    public string dummy; //더미 변수 - 컴포넌트가 사라지는 것을 방지하기 위해 사용

    RectTransform rect; //RectTransform 컴포넌트

    /// <summary>
    /// 시작 시 호출 - 컴포넌트 초기화
    /// </summary>
    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }


    /// <summary>
    /// 물리 업데이트 - 플레이어 위치에 따라 UI 이동
    /// </summary>
    void FixedUpdate()
    {
        // Check if GameManager and player exist to avoid MissingReferenceException
        if (GameManager.instance == null || GameManager.instance.player == null)
        {
            // Skip update if player doesn't exist
            return;
        }

        //타겟의 월드 좌표를 스크린 좌표로 변환하여 UI 위치 설정
        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
