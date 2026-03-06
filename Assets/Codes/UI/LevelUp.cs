using UnityEngine;

/// <summary>
/// 레벨업 클래스 - 레벨업 시 선택지 UI를 표시합니다.
/// </summary>
public class LevelUp : MonoBehaviour
{

    RectTransform rect; //RectTransform
    Item[] items; //아이템 배열

    /// <summary>
    /// 시작 시 호출 - 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    /// <summary>
    /// 레벨업 UI 표시
    /// </summary>
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.SFX.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }


    /// <summary>
    /// 레벨업 UI 숨기기
    /// </summary>
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);
        AudioManager.instance.EffectBgm(false);
    }

    /// <summary>
    /// 초기 무기 선택 - 게임 시작 시 호출
    /// </summary>
    /// <param name="index">선택한 무기 인덱스</param>
    public void Select(int index)
    {
        items[index].OnClickItem();
    }

    /// <summary>
    /// 다음 레벨업 아이템 선택 - 랜덤 3개 아이템 표시
    /// </summary>
    void Next()
    {

        //1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        //2. 그중에 랜덤 3개 아이템 활성화
        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            //중복되지 않게
            if (ran[0] != ran[1] && ran[0] != ran[2] && ran[1] != ran[2])
            {
                break;
            }

        }

        //3. 만렙 아이템의 경우는 소비아이템으로 대체
        for (int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];
            if (ranItem.level == ranItem.data.damages.Length)
            {
                items[4].gameObject.SetActive(true);    //음료수
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }

        }
    }


}
