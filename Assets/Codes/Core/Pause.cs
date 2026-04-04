using UnityEngine;

public class Pause : MonoBehaviour
{
    RectTransform rect; //RectTransform
    Player player; //플레이어 참조

    /// <summary>
    /// 시작 시 호출 - 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        player = GameManager.instance.player;
    }

    /// <summary>
    /// 레벨업 UI 표시
    /// </summary>
    public void Show()
    {
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        // AudioManager.instance.PlaySfx(AudioManager.SFX.LevelUp);
        // AudioManager.instance.EffectBgm(true);
    }


    /// <summary>
    /// 레벨업 UI 숨기기
    /// </summary>
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        // AudioManager.instance.PlaySfx(AudioManager.SFX.Select);
        // AudioManager.instance.EffectBgm(false);
    }

    public void Lobby()
    {

        GameManager.instance.Lobby();
    }


}
