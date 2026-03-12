using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 레벨업 클래스 - 레벨업 시 선택지 UI를 표시합니다.
/// </summary>
public class LevelUp : MonoBehaviour
{

    RectTransform rect; //RectTransform
    Item[] items; //아이템 배열
    Player player; //플레이어 참조

    /// <summary>
    /// 시작 시 호출 - 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
        player = GameManager.instance.player;
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

        //2. 스탬팩이 활성화 중이면 스탬팩 아이템을 제외하고 랜덤 3개 아이템 선택
        List<int> candidates = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            Item item = items[i];
            // 스탬팩이 활성화 중이면 스탬팩 아이템 제외
            if (item.data.itemType == ItemData.ItemType.StamPack && player != null && player.IsStampPackActive)
            {
                continue;
            }
            candidates.Add(i);
        }

        int[] ran = new int[3];
        int count = candidates.Count;
        if (count >= 3)
        {
            // 3개 이상의 후보가 있을 때, 중복되지 않게 3개 선택
            int a = Random.Range(0, count);
            int b = Random.Range(0, count);
            while (b == a) b = Random.Range(0, count);
            int c = Random.Range(0, count);
            while (c == a || c == b) c = Random.Range(0, count);
            ran[0] = candidates[a];
            ran[1] = candidates[b];
            ran[2] = candidates[c];
        }
        else
        {
            // 후보가 3개 미만이면 가능한 만큼 선택 (부족한 경우 첫 번째 아이템으로 채움)
            for (int i = 0; i < 3; i++)
            {
                if (i < count)
                {
                    ran[i] = candidates[i];
                }
                else
                {
                    ran[i] = 0; // fallback to first item if no candidates (should not happen in normal gameplay)
                }
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
