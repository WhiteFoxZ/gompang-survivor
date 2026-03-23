using UnityEngine;

/// <summary>
/// 게임 아이템 데이터 클래스 - ScriptableObject로 아이템 속성을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/ShopItem", order = 1)]
public class ShopItemSO : ScriptableObject
{

    public ShopItemType shopItemType; //상점 아이템 유형
    public PayType payType; //결제 유형


    public string title;
    public int price; //가격
    public int itemCnt; //충천갯수    


    public Sprite ImageItem;
    public Sprite ImageAd;



}

/// <summary>
/// 상점아이템타입
/// </summary>
public enum ShopItemType
{
    Energy, //에너지
    Coin, //코인
    Diamond,    //다이아
    ItemBoxCommon,
    ItemBoxRare,
    ItemBoxEpic,
    ItemBoxLegendary
}


public enum PayType
{
    AD, //광고
    COIN, //코인
    DIAMOND,//다이아몬드
    PAY //코인
}

