using UnityEngine;

/// <summary>
/// 따라다니는 UI 클래스 - 오브젝트의 위치에 따라 UI를 이동시킵니다.
/// </summary>
public class FollowUi : MonoBehaviour
{
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
        //타겟의 월드 좌표를 스크린 좌표로 변환하여 UI 위치 설정
        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
