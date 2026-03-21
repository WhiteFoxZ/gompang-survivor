using UnityEngine;
using System.Collections.Generic;

public class RandomSelect : MonoBehaviour
{

    [Header("보통아이템 1,2,3,4")]
    public List<GameItem> gameItemCommon;//보통템 흰색

    [Header("레어아이템 3,4,5,6")]
    public List<GameItem> gameItemRare; //레어템  파란색

    [Header("에픽아이템 6,7,8")]
    public List<GameItem> gameItemEpic; //에픽템  보라섹

    [Header("전설아이템 8,9,10")]
    public List<GameItem> gameItemLegendary;//전설템  노락색



    //덱이미지를 보여줄
    [Header("아이템이미지")]
    public GameObject _scrollviewContents;

    // 프리펩들을 보관할 변수
    public GameObject _prefabs;


    //보통템 
    public GameItem getGameItemCommon()
    {

        return RandomCard(gameItemCommon);
    }

    //레어템 
    public GameItem getGameItemRare()
    {
        return RandomCard(gameItemRare);
    }

    //에픽템  
    public GameItem getgameItemEpic()
    {
        return RandomCard(gameItemEpic);
    }

    //전설템  
    public GameItem getgameItemLegendary()
    {
        return RandomCard(gameItemLegendary);
    }



    // 가중치 랜덤의 설명은 영상을 참고.
    private GameItem RandomCard(List<GameItem> deck)
    {
        int total = 0;  // 카드들의 가중치 총 합

        for (int i = 0; i < deck.Count; i++)
        {
            // 스크립트가 활성화 되면 카드 덱의 모든 카드의 총 가중치를 구해줍니다.
            total += deck[i].weight;
        }

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
