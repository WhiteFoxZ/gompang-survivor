using UnityEngine;
using System.Collections.Generic;

public class RandomSelect : MonoBehaviour
{

    public List<GameItem> deck = new List<GameItem>();  // 카드 덱
    public int total = 0;  // 카드들의 가중치 총 합

    public List<GameItem> result = new List<GameItem>();  // 랜덤하게 선택된 카드를 담을 리스트


    void Start()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            // 스크립트가 활성화 되면 카드 덱의 모든 카드의 총 가중치를 구해줍니다.
            total += deck[i].weight;
        }
        // 실행
        ResultSelect();
    }

    public void ResultSelect()
    {
        for (int i = 0; i < 10; i++)
        {
            // 가중치 랜덤을 돌리면서 결과 리스트에 넣어줍니다.
            result.Add(RandomCard());
            // 비어 있는 카드를 생성하고
        }
    }

    // 가중치 랜덤의 설명은 영상을 참고.
    public GameItem RandomCard()
    {
        int weight = 0;
        int selectNum = 0;

        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

        for (int i = 0; i < deck.Count; i++)
        {
            weight += deck[i].weight;
            if (selectNum <= weight)
            {
                GameItem temp = new GameItem(deck[i]);
                return temp;
            }
        }
        return null;
    }





}
